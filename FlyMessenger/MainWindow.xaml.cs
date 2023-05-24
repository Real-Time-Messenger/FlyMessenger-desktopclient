using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using FlyMessenger.Core;
using FlyMessenger.Core.Utils;
using FlyMessenger.MVVM.Model;
using FlyMessenger.MVVM.ViewModels;
using FlyMessenger.Properties;
using FlyMessenger.Resources.Settings;
using Microsoft.Win32;
using Newtonsoft.Json;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using ListViewItem = System.Windows.Controls.ListViewItem;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace FlyMessenger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        // NotifyIconManager handler
        public WebSockets WebSocketClient;
        private MainWindow _mainWindow;
        public string LangSwitch { get; set; } = "";
        public static MainViewModel MainViewModel { get; private set; } = new MainViewModel();
        private readonly NotifyIconManager _notifyIconManager = new NotifyIconManager();
        private bool _searched;
        public bool IsUpdatingView = false;

        /// <summary>
        /// Constructor for MainWindow
        /// </summary>
        public MainWindow()
        {
            var langCode = Settings.Default.LanguageCode;
            Thread.CurrentThread.CurrentUICulture = langCode != "" ? new CultureInfo(langCode) : new CultureInfo("");

            Initialized += MainWindowInitialized;
            InitializeComponent();

            App.NavBarProfilePhoto = ProfilePhoto;
            PreviewKeyDown += KeyDisable;
            Loaded += MainWindowLoaded;
            Closed += (sender, args) =>
            {
                App.ToggleLanguage(sender, args);
                _notifyIconManager.DisposeNotifyIcon();
            };

            Activated += (_, _) =>
            {
                WebSocketClient?.Send(
                    JsonConvert.SerializeObject(
                        new ToggleOnlineStatus
                        {
                            type = "TOGGLE_ONLINE_STATUS",
                            status = true
                        }
                    )
                );
            };

            Deactivated += (_, _) =>
            {
                WebSocketClient?.Send(
                    JsonConvert.SerializeObject(
                        new ToggleOnlineStatus
                        {
                            type = "TOGGLE_ONLINE_STATUS",
                            status = false
                        }
                    )
                );
            };

            MainViewModel = new MainViewModel();
            _notifyIconManager.InitializeNotifyIcon();
        }

        /// <summary>
        /// Initialized event for MainWindow
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void MainWindowInitialized(object? sender, EventArgs e)
        {
            WebSocketClient = new WebSockets();
        }

        /// <summary>
        /// Loaded event for MainWindow
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = MainViewModel;
            MainViewModel.ActiveChatVisHidden = true;
            MainViewModel.ActiveChatVisibility = false;

            var setOnlineStatus = new ToggleOnlineStatus
            {
                type = "TOGGLE_ONLINE_STATUS",
                status = true
            };
            WebSocketClient.Send(JsonConvert.SerializeObject(setOnlineStatus));
        }

        /// <summary>
        /// Check if object is in viewport
        /// </summary>
        /// <param name="element">Object to check</param>
        /// <returns>True if object is in viewport</returns>
        private static bool IsInViewport(UIElement element)
        {
            // Get the ScrollViewer parent of the element
            var scrollViewer = FindParent<ScrollViewer>(element);
            if (scrollViewer == null)
                return false;

            // Get the location of the element relative to the ScrollViewer
            var elementRect = element.TransformToAncestor(scrollViewer)
                .TransformBounds(new Rect(0, 0, element.RenderSize.Width, element.RenderSize.Height));
            // Get the location of the viewport relative to the ScrollViewer
            var viewportRect = new Rect(0, 0, scrollViewer.ActualWidth, scrollViewer.ActualHeight);

            // Check if the element is in the viewport
            return viewportRect.IntersectsWith(elementRect);
        }

        /// <summary>
        /// Find parent of object
        /// </summary>
        /// <param name="child">Object to find parent</param>
        /// <typeparam name="T">Type of parent</typeparam>
        /// <returns>Parent of object</returns>
        private static T? FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            while (true)
            {
                var parent = VisualTreeHelper.GetParent(child);
                switch (parent)
                {
                    case null:
                        return null;
                    case T parentAsT:
                        return parentAsT;
                    default:
                        child = parent;
                        break;
                }
            }
        }

        /// <summary>
        /// Check if message is on screen
        /// </summary>
        public void CheckMessageOnScreen()
        {
            foreach (var item in _mainWindow.MessagesListView.Items)
            {
                if (MessagesListView.ItemContainerGenerator.ContainerFromItem(item) is not ListViewItem listViewItem) continue;
                if (!IsInViewport(listViewItem)) continue;
                var message = (MessageModel)listViewItem.Content;
                if (message.IsRead || message.IsMyMessage == true || message.IsMyPhotoVisible == true) continue;
                var readMessage = new ReadMessageModel
                {
                    type = "READ_MESSAGE",
                    dialogId = MainViewModel.SelectedDialog.Id,
                    messageId = message.Id
                };

                WebSocketClient.Send(JsonConvert.SerializeObject(readMessage));
                message.IsRead = true;
                MainViewModel.SelectedDialog.UnreadMessages -= 1;
                var selectedDialog = MainViewModel.SelectedDialog;
                selectedDialog.UnreadMessages = Math.Max(selectedDialog.UnreadMessages - 1, 0);
                MainViewModel.UnreadMessagesCount =
                    MainViewModel.Dialogs.Count(x => x.UnreadMessages > 0);
                ChatBoxListView.Items.Refresh();
            }

            if (Application.Current.MainWindow is not MainWindow { IsActive: true }) return;
            NotificationsManager.CloseDialogNotifications(MainViewModel.SelectedDialog.Id);
        }

        /*/// <summary>
        /// Get first item in viewport
        /// </summary>
        /// <returns>First item in viewport</returns>
        private UIElement? GetFirstItemInViewport()
        {
            // Get the last item that isInViewport
            var firstItem = _mainWindow.MessagesListView.Items.Cast<object>().FirstOrDefault(IsInViewport);
            if (firstItem == null) return null;
            var listViewItem = (ListViewItem)_mainWindow.MessagesListView.ItemContainerGenerator.ContainerFromItem(firstItem);
            return listViewItem;
        }*/

        /// <summary>
        /// Disable non usable keys
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private static void KeyDisable(object sender, KeyEventArgs e)
        {
            if (e.Key is Key.Tab or Key.LeftAlt)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// KeyDown event for TopBar Close button
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void TopBarCloseButton(object sender, MouseButtonEventArgs e)
        {
            Hide();
        }

        /// <summary>
        /// Drag MainWindow when mouse is down on TopBar
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void DragMainWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        /// <summary>
        /// Mouse click event to TopBar Maximize button
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void TopBarMaximizeButton(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        /// <summary>
        /// Mouse click event to TopBar Minimize button
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void TopBarMinimizeButton(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Mouse enter event on SideBar
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void SideBarMouseEnter(object sender, MouseEventArgs e)
        {
            var borderVisibility = MainViewModel.BorderVisibility;

            if (borderVisibility) return;
            var animation = new DoubleAnimation(
                DarkBorder.Opacity,
                0.25,
                TimeSpan.FromSeconds(0.2)
            );

            MainViewModel.BorderVisibility = true;

            DarkBorder.BeginAnimation(OpacityProperty, animation);
        }

        /// <summary>
        /// Mouse leave event on SideBar
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void SideBarMouseLeave(object sender, MouseEventArgs e)
        {
            var borderVisibility = MainViewModel.BorderVisibility;

            if (!borderVisibility) return;
            var animation = new DoubleAnimation(
                DarkBorder.Opacity,
                0,
                TimeSpan.FromSeconds(0.2)
            );

            animation.Completed += (_, _) => MainViewModel.BorderVisibility = false;

            DarkBorder.BeginAnimation(OpacityProperty, animation);
        }

        /// <summary>
        /// Handle mouse click on Logout button
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void LogoutButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (LogoutModalWindow.IsOpen) return;

            LogoutModalWindow.IsOpen = true;

            var openAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.2));

            LogoutModalWindow.BeginAnimation(OpacityProperty, openAnimation);
        }

        /// <summary>
        /// Handle mouse click on Conservations button
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void ConservationsButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (NotImplementedTip.IsOpen) return;

            NotImplementedTip.IsOpen = true;

            var openAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.2));
            var closeAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.2))
            {
                BeginTime = TimeSpan.FromSeconds(2)
            };

            openAnimation.Completed += (_, _) => NotImplementedTip.BeginAnimation(OpacityProperty, closeAnimation);
            closeAnimation.Completed += (_, _) => NotImplementedTip.IsOpen = false;

            NotImplementedTip.BeginAnimation(OpacityProperty, openAnimation);
        }

        /// <summary>
        /// Handle mouse click on Groups button
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void GroupsButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (NotImplementedTip.IsOpen) return;

            NotImplementedTip.IsOpen = true;

            var openAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.2));
            var closeAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.2))
            {
                BeginTime = TimeSpan.FromSeconds(2)
            };

            openAnimation.Completed += (_, _) => NotImplementedTip.BeginAnimation(OpacityProperty, closeAnimation);
            closeAnimation.Completed += (_, _) => NotImplementedTip.IsOpen = false;

            NotImplementedTip.BeginAnimation(OpacityProperty, openAnimation);
        }

        /// <summary>
        /// Handle mouse click on Settings button
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void SettingsButtonClick(object sender, MouseButtonEventArgs e)
        {
            ModalWindow.IsOpen = true;

            // Create animation that will be used to show modal window with opacity and slide from right to left
            var animation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.2));
            var slideAnimation = new ThicknessAnimation(
                new Thickness(0, 0, -150, 0),
                new Thickness(0, 0, 0, 0),
                TimeSpan.FromSeconds(0.2)
            );

            // Start animation
            ModalWindow.BeginAnimation(OpacityProperty, animation);
            ModalWindowBorder.BeginAnimation(MarginProperty, slideAnimation);
        }

        /// <summary>
        /// Handle mouse click on Support button
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void SupportButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (NotImplementedTip.IsOpen) return;

            NotImplementedTip.IsOpen = true;

            var openAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.2));
            var closeAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.2))
            {
                BeginTime = TimeSpan.FromSeconds(2)
            };

            openAnimation.Completed += (_, _) => NotImplementedTip.BeginAnimation(OpacityProperty, closeAnimation);
            closeAnimation.Completed += (_, _) => NotImplementedTip.IsOpen = false;

            NotImplementedTip.BeginAnimation(OpacityProperty, openAnimation);
        }

        /// <summary>
        /// Handle mouse click on Theme button
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void ThemeButtonClick(object sender, MouseButtonEventArgs e)
        {
            var resources = Application.Current.Resources.MergedDictionaries;

            // Clear all resources
            resources.Clear();

            // Add new resources
            if (Settings.Default.CurrentTheme == "Light")
            {
                resources.Add(
                    new ResourceDictionary
                    {
                        Source = new Uri("pack://application:,,,/Resources/Colors/Dark.xaml")
                    }
                );

                Settings.Default.CurrentTheme = "Dark";
                Settings.Default.Save();
            }
            else
            {
                resources.Add(
                    new ResourceDictionary
                    {
                        Source = new Uri("pack://application:,,,/Resources/Colors/Light.xaml")
                    }
                );

                Settings.Default.CurrentTheme = "Light";
                Settings.Default.Save();
            }
        }

        /// <summary>
        /// Handle mouse click on Search button in active chat
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void ActiveChatSearch(object sender, MouseButtonEventArgs e)
        {
            SearchBox.Visibility = Visibility.Collapsed;
            SearchBoxSecond.Visibility = Visibility.Visible;
            SearchInChatPanel.Visibility = Visibility.Visible;
            ClearSearchBoxButton.Visibility = Visibility.Visible;
            DialogsPanel.Visibility = Visibility.Collapsed;

            // Set focus to SearchBox
            SearchBoxSecond.Focus();

            MainViewModel.SelectedDialogSearch =
                new DialogInSearchModel
                {
                    Id = MainViewModel.SelectedDialog.Id,
                    FirstName = MainViewModel.SelectedDialog.User.FirstName,
                    LastName = MainViewModel.SelectedDialog.User.LastName,
                    PhotoUrl = MainViewModel.SelectedDialog.User.PhotoUrl
                };
        }

        /// <summary>
        /// Handle mouse click on modal window close button
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void ModalClose(object sender, RoutedEventArgs e)
        {
            var animation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.1));

            animation.Completed += (_, _) =>
            {
                ModalWindow.IsOpen = false;

                SettingsManager.GoBack();
            };

            ModalWindow.BeginAnimation(OpacityProperty, animation);
        }

        /// <summary>
        /// Handle mouse click on language modal window close button
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void LanguageModalClose(object sender, RoutedEventArgs e)
        {
            var animation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.1));

            animation.Completed += (_, _) => { LanguageModalWindow.IsOpen = false; };

            LanguageModalWindow.BeginAnimation(OpacityProperty, animation);
        }

        /// <summary>
        /// Handle mouse click on settings modal window close button
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void SettingsProfile(object sender, MouseButtonEventArgs e)
        {
            SettingsManager.OpenPage(1);
            ModalWindow.IsOpen = true;

            // Create animation that will be used to show modal window with opacity and slide from right to left
            var animation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.2));
            var slideAnimation = new ThicknessAnimation(
                new Thickness(0, 0, -150, 0),
                new Thickness(0, 0, 0, 0),
                TimeSpan.FromSeconds(0.2)
            );

            // Start animation
            ModalWindow.BeginAnimation(OpacityProperty, animation);
            ModalWindowBorder.BeginAnimation(MarginProperty, slideAnimation);
        }

        /// <summary>
        /// Handle mouse click on back button in settings modal window
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void GoBack(object sender, RoutedEventArgs e)
        {
            SettingsManager.GoBack();
        }

        /// <summary>
        /// Handle mouse click on LastActivity modal window close button
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void LastActivityModalClose(object sender, RoutedEventArgs e)
        {
            var animation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.1));

            animation.Completed += (_, _) => { LastActivityModalWindow.IsOpen = false; };

            LastActivityModalWindow.BeginAnimation(OpacityProperty, animation);
        }

        /// <summary>
        /// Handle mouse click on dialog in dialogs list
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void OnSelectedChat(object sender, RoutedEventArgs e)
        {
            MainViewModel.ActiveChatVisHidden = false;
            MainViewModel.ActiveChatVisibility = true;
            
            var selectedItem = DialogsInSearch.SelectedItem as DialogModel;
            var lastChats = MainViewModel.LastItemsInSearch.Dialogs;
            
            // Insert selected object to LastItemsInSearch collection with max size 3
            if (lastChats.Contains(selectedItem))
                lastChats.Remove(selectedItem);
            
            if (SearchBox.Text != "")
                LimitedSizeStack.Push(selectedItem, false, true);

            if (MessagesListView.Items.IsEmpty) return;
            CheckMessageOnScreen();
            // If I have IsRead = false, then scroll to first unread message, else scroll to last message
            var firstUnreadMessage =
                MainViewModel.SelectedDialog.Messages.FirstOrDefault(
                    x => x.IsRead == false && x.Sender.Id != MainViewModel.MyProfile.Id
                );
            MessagesListView.ScrollIntoView(firstUnreadMessage ?? MainViewModel.SelectedDialog.Messages.Last());
        }

        /// <summary>
        /// Handle mouse click on user in users search list
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void OnSelectedUser(object sender, MouseButtonEventArgs e)
        {
            MainViewModel.ActiveChatVisHidden = false;
            MainViewModel.ActiveChatVisibility = true;
            if (MainViewModel.NoMessagesVisibility) MainViewModel.NoMessagesVisibility = false;

            var selectedItem = UsersInSearch.SelectedItem as DialogModel;
            var lastUsers = MainViewModel.LastItemsInSearch.Users;
            
            // Insert selected object to LastItemsInSearch collection with max size 3
            if (lastUsers.Contains(selectedItem))
                lastUsers.Remove(selectedItem);
            
            if (SearchBox.Text != "")
                LimitedSizeStack.Push(selectedItem, false, true);
            
            // Rewrite selected dialog to LastItemsInSearch collection dialog
            var dialog = MainViewModel.Dialogs.FirstOrDefault(x => x.User.Id == selectedItem.User.Id);
            MainViewModel.SelectedDialog = dialog ?? selectedItem;
            
            if (MessagesListView.Items.IsEmpty) return;
            CheckMessageOnScreen();
            var firstUnreadMessage =
                MainViewModel.SelectedDialog.Messages.FirstOrDefault(
                    x => x.IsRead == false && x.Sender.Id != MainViewModel.MyProfile.Id
                );
            MessagesListView.ScrollIntoView(firstUnreadMessage ?? MainViewModel.SelectedDialog.Messages.Last());
        }

        /// <summary>
        /// Handle mouse click on message in messages search list
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void OnSelectedMessage(object sender, MouseButtonEventArgs e)
        {
            MainViewModel.ActiveChatVisHidden = false;
            MainViewModel.ActiveChatVisibility = true;

            // Insert selected dialog to new collection
            var selectedItem = MessagesInSearch.SelectedItem as DialogModel;
            var lastMessages = MainViewModel.LastItemsInSearch.Messages;

            // Insert selected object to LastItemsInSearch collection with max size 3
            if (lastMessages.Contains(selectedItem))
                lastMessages.Remove(selectedItem);
            
            LimitedSizeStack.Push(selectedItem, false, false);

            // Rewrite selected dialog to LastItemsInSearch collection dialog
            var dialog = MainViewModel.Dialogs.FirstOrDefault(
                x => x.User.Id == selectedItem.User.Id
            );
            MainViewModel.SelectedDialog = dialog!;
            
            // Check if message is in selected dialog
            if (MessagesListView.Items.IsEmpty) return;
            CheckMessageOnScreen();
            
            // Scroll to selected message
            foreach (var message in dialog.Messages)
            {
                if (message.Id != selectedItem.LastMessage?.Id) continue;
                MessagesListView.ScrollIntoView(message);
                break;
            }
        }

        /// <summary>
        /// Handle mouse click on item in last items search list
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void OnSelectedLastItem(object sender, MouseButtonEventArgs e)
        {
            MainViewModel.ActiveChatVisHidden = false;
            MainViewModel.ActiveChatVisibility = true;

            // Open chat
            var dialog = MainViewModel.Dialogs.FirstOrDefault(x => x.User.Id == MainViewModel.SelectedDialog.User.Id);
            if (dialog == null) return;
            MainViewModel.SelectedDialog = dialog;
        }

        /// <summary>
        /// Handle scroll changed event in active chat
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void HandleScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            _mainWindow = this;
            if (MainViewModel.SelectedDialog == null!) return;
            CheckMessageOnScreen();
            MainViewModel.ScrollDownButtonVisibility = e.VerticalOffset < e.ExtentHeight - e.ViewportHeight - 320;
            if (e.VerticalOffset is > 48 or 0) return;
            MainViewModel.LoadMoreItems();
        }

        /// <summary>
        /// Handle scroll down button click
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void ScrollDownButtonClick(object sender, MouseButtonEventArgs e)
        {
            MessagesListView.ScrollIntoView(MainViewModel.SelectedDialog.Messages.Last());

            foreach (var item in MessagesListView.Items)
            {
                var message = (MessageModel)item;
                if (message.IsRead || message.IsMyMessage == true) continue;
                var readMessage = new ReadMessageModel
                {
                    type = "READ_MESSAGE",
                    dialogId = MainViewModel.SelectedDialog.Id,
                    messageId = message.Id
                };

                WebSocketClient.Send(JsonConvert.SerializeObject(readMessage));
                message.IsRead = true;
            }
        }

        /// <summary>
        /// Close window event
        /// </summary>
        public void CloseWindow()
        {
            Close();
            NotificationsManager.CloseAllNotifications();
            _notifyIconManager.DisposeNotifyIcon();
        }

        private CancellationTokenSource? _cancellationTokenSource;

        /// <summary>
        /// Handle search box click
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private async void SearchBoxClick(object sender, KeyEventArgs e)
        {
            // Delete old token
            _cancellationTokenSource?.Cancel();
            if (e.Key != Key.Enter)
            {
                _searched = false;
            }

            // Create new token
            _cancellationTokenSource = new CancellationTokenSource();

            ClearSearchBoxButton.Visibility =
                string.IsNullOrWhiteSpace(SearchBox.Text) ? Visibility.Collapsed : Visibility.Visible;
            DialogsPanel.Visibility =
                string.IsNullOrWhiteSpace(SearchBox.Text) ? Visibility.Visible : Visibility.Collapsed;
            SearchAllPanel.Visibility =
                string.IsNullOrWhiteSpace(SearchBox.Text) ? Visibility.Collapsed : Visibility.Visible;
            SearchInChatPanel.Visibility = Visibility.Collapsed;

            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                MainViewModel.DialogsInSearch.Clear();
                MainViewModel.UsersInSearch.Clear();
                MainViewModel.MessagesInSearch.Clear();
                _searched = false;
                return;
            }
            if (e.Key == Key.Enter && _searched == false)
            {
                _searched = true;
                await MainViewModel.Search(SearchBox.Text);
            }
            if (!IsTextKey(e.Key)) return;

            try
            {
                // Wait 1 second before search
                await Task.Delay(TimeSpan.FromSeconds(1), _cancellationTokenSource.Token);
                if (_searched) return;
                await MainViewModel.Search(SearchBox.Text);
                _searched = true;
            }
            catch (TaskCanceledException)
            {
                // Do nothing
            }
        }

        private async void SearchBoxSecondClick(object sender, KeyEventArgs e)
        {
            // Delete old token
            _cancellationTokenSource?.Cancel();

            // Create new token
            _cancellationTokenSource = new CancellationTokenSource();

            ClearSearchBoxButton.Visibility = Visibility.Visible;
            DialogsPanel.Visibility = Visibility.Collapsed;
            SearchInChatPanel.Visibility = Visibility.Visible;
            SearchAllPanel.Visibility = Visibility.Collapsed;

            if (string.IsNullOrWhiteSpace(SearchBoxSecond.Text))
            {
                MainViewModel.MessagesInSearch.Clear();
                return;
            }
            if (e.Key == Key.Enter) await MainViewModel.SearchInDialog(SearchBoxSecond.Text);
            if (!IsTextKey(e.Key)) return;

            try
            {
                // Wait 1 second before search
                await Task.Delay(TimeSpan.FromSeconds(1), _cancellationTokenSource.Token);
                await MainViewModel.SearchInDialog(SearchBoxSecond.Text);
            }
            catch (TaskCanceledException)
            {
                // Ignore, if the task was canceled
            }
        }

        /// <summary>
        /// Check if the key is a letter or a digit
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>True if the key is a letter or a digit, false otherwise</returns>
        private static bool IsTextKey(Key key)
        {
            // Check if the key is a letter or a digit
            return key is >= Key.A and <= Key.Z or >= Key.D0 and <= Key.D9 or >= Key.NumPad0 and <= Key.NumPad9
                or >= Key.Oem1 and <= Key.OemClear;
        }

        /// <summary>
        /// Select file event
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void SelectFile(object sender, MouseButtonEventArgs e)
        {
            // Open file dialog to select file
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg;*.gif)|*.png;*.jpeg;*.jpg;*.gif",
                Multiselect = false,
                Title = "Select a file"
            };

            if (openFileDialog.ShowDialog() != true) return;

            // Get file info
            var fileName = openFileDialog.FileName;
            if (string.IsNullOrWhiteSpace(fileName)) return;
            var fileInfo = new FileInfo(fileName);
            var fileSize = fileInfo.Length;

            // Validate file
            if (fileSize > 5242880)
            {
                MessageBox.Show(
                    "File size is too big. Max file size is 5MB",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }

            var fileExtension = Path.GetExtension(fileName);
            var fileExtensionLower = fileExtension.ToLower();
            if (fileExtensionLower != ".png" && fileExtensionLower != ".jpeg" && fileExtensionLower != ".jpg" &&
                fileExtensionLower != ".gif")
            {
                MessageBox.Show(
                    "File type is not supported. Supported file types are: png, jpeg, jpg, gif",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }

            // Convert file to base64
            var fileBytes = File.ReadAllBytes(fileName);
            var fileBase64 = Convert.ToBase64String(fileBytes);
            var selectedDialog = MainViewModel.SelectedDialog;

            // Send file to the selected dialog via WebSocket
            var fileModel = new SendMessageModel
            {
                type = "SEND_MESSAGE",
                dialogId = selectedDialog.Id,
                file = new FileModel
                {
                    name = Path.GetFileName(fileName),
                    size = fileSize,
                    type = fileExtensionLower,
                    data = fileBase64
                },
                recipientId = selectedDialog.User.Id
            };

            WebSocketClient.Send(JsonConvert.SerializeObject(fileModel));
        }

        /// <summary>
        /// Send message event
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void SendMessage(object sender, MouseButtonEventArgs e)
        {
            // Send typing status to the selected dialog via WebSocket
            var typingState = new SendMessageModel
            {
                type = string.IsNullOrWhiteSpace(ActiveChatMessage.Text) ? "UNTYPING" : "TYPING",
                dialogId = ((MainViewModel)DataContext).SelectedDialog.Id,
            };

            var untypingState = new SendMessageModel
            {
                type = "UNTYPING",
                dialogId = ((MainViewModel)DataContext).SelectedDialog.Id,
            };

            WebSocketClient.Send(JsonConvert.SerializeObject(typingState));

            var text = ActiveChatMessage.Text;
            ActiveChatMessage.Text = string.Empty;

            if (string.IsNullOrWhiteSpace(text)) return;
            text = text.Trim();

            var message = new SendMessageModel
            {
                type = "SEND_MESSAGE",
                file = null,
                dialogId = MainViewModel.SelectedDialog.Id,
                recipientId = MainViewModel.SelectedDialog.User.Id
            };

            // Cut message to 1000 characters and send it via WebSocket
            if (text.Length > 1000)
            {
                for (var i = 0; i < text.Length; i += 1000)
                {
                    message.text = text.Substring(i, Math.Min(1000, text.Length - i));
                    WebSocketClient.Send(JsonConvert.SerializeObject(message));
                }
            }
            else
            {
                message.text = text;
                WebSocketClient.Send(JsonConvert.SerializeObject(message));
                WebSocketClient.Send(JsonConvert.SerializeObject(untypingState));
            }
        }
        
        /// <summary>
        /// Handle input in active chat message input field
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void OnReleaseKeyUp(object? sender, KeyEventArgs e)
        {
            // Send typing status to the selected dialog via WebSocket
            var typingState = new SendMessageModel
            {
                type = string.IsNullOrWhiteSpace(ActiveChatMessage.Text) ? "UNTYPING" : "TYPING",
                dialogId = ((MainViewModel)DataContext).SelectedDialog.Id,
            };

            var untypingState = new SendMessageModel
            {
                type = "UNTYPING",
                dialogId = ((MainViewModel)DataContext).SelectedDialog.Id,
            };

            WebSocketClient.Send(JsonConvert.SerializeObject(typingState));

            // Handle the enter key press event in the chat input field
            switch (e.Key)
            {
                case Key.Enter when (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift:
                    ActiveChatMessage.Text += Environment.NewLine;
                    ActiveChatMessage.CaretIndex = ActiveChatMessage.Text.Length;
                    break;
                case Key.Enter:
                    var text = ActiveChatMessage.Text;
                    ActiveChatMessage.Text = string.Empty;

                    if (string.IsNullOrWhiteSpace(text)) return;
                    text = text.Trim();

                    var message = new SendMessageModel
                    {
                        type = "SEND_MESSAGE",
                        file = null,
                        dialogId = MainViewModel.SelectedDialog.Id,
                        recipientId = MainViewModel.SelectedDialog.User.Id,
                    };

                    if (text.Length > 1000)
                    {
                        for (var i = 0; i < text.Length; i += 1000)
                        {
                            message.text = text.Substring(i, Math.Min(1000, text.Length - i));
                            WebSocketClient.Send(JsonConvert.SerializeObject(message));
                        }
                    }
                    else
                    {
                        message.text = text;
                        WebSocketClient.Send(JsonConvert.SerializeObject(message));
                        WebSocketClient.Send(JsonConvert.SerializeObject(untypingState));
                    }
                    break;
                default:
                    return;
            }
        }
    }
}
