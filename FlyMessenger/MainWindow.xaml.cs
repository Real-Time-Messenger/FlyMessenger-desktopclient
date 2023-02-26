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
using FlyMessenger.Controllers;
using FlyMessenger.Core;
using FlyMessenger.Core.Models;
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
        public static MainViewModel MainViewModel { get; } = new MainViewModel();
        private readonly NotifyIconManager _notifyIconManager = new NotifyIconManager();

        public MainWindow()
        {
            var langCode = Settings.Default.LanguageCode;
            Thread.CurrentThread.CurrentUICulture = langCode != "" ? new CultureInfo(langCode) : new CultureInfo("");

            Initialized += MainWindow_Initialized;
            InitializeComponent();

            App.ProfilePhotoMainWindow = ProfilePhoto;
            PreviewKeyDown += MainWindowPreviewKeyDown;
            Loaded += MainWindow_Loaded;
            Closed += App.ToggleLanguage;
            
            _notifyIconManager.InitializeNotifyIcon();
        }

        private void MainWindow_Initialized(object? sender, EventArgs e)
        {
            WebSocketClient = new WebSockets();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = MainViewModel;
            MainViewModel.ActiveChatVisibilityNone = true;
            MainViewModel.ActiveChatVisibility = false;

            var setOnlineStatus = new ToggleOnlineStatus
            {
                type = "TOGGLE_ONLINE_STATUS",
                status = true
            };
            WebSocketClient.Send(JsonConvert.SerializeObject(setOnlineStatus));
        }

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

        public void MainWindow_CheckMessageOnScreen()
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
                    MainViewModel.Dialogs!.Count(x => x.UnreadMessages > 0);
                ChatBoxListView.Items.Refresh();
            }
        }

        private UIElement? MainWindow_GetFirstItemIsInViewport()
        {
            // Get the last item that isInViewport
            var firstItem = _mainWindow.MessagesListView.Items.Cast<object>().FirstOrDefault(IsInViewport);
            if (firstItem == null) return null;
            var listViewItem = (ListViewItem)_mainWindow.MessagesListView.ItemContainerGenerator.ContainerFromItem(firstItem);
            return listViewItem;
        }

        private static void MainWindowPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key is Key.Tab or Key.LeftAlt)
            {
                e.Handled = true;
            }
        }

        // Mouse click event to TopBar Close button
        private void TopBarCloseButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Hide();
        }

        // Mouse hold event
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        // Mouse up event to TopBar Maximize button
        private void TopBarMaximizeButton_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        // Mouse enter event to TopBar Minimize button
        private void TopBarMinimizeButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void SideBar_OnMouseEnter(object sender, MouseEventArgs e)
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

        private void SideBar_OnMouseLeave(object sender, MouseEventArgs e)
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

        // Mouse left button up event to change IsActive property
        private void LogoutChatMenuButton_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (LogoutModalWindow.IsOpen) return;
            
            LogoutModalWindow.IsOpen = true;
            
            var openAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.2));
            
            LogoutModalWindow.BeginAnimation(OpacityProperty, openAnimation);
        }

        private void ConservationsChatMenuButton_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
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

        private void GroupsChatMenuButton_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
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

        private void SettingsChatMenuButton_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
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

        private void SupportChatMenuButton_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
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

        private void ThemeChatMenuButton_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
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

        private void ActiveChat_Search_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SearchBox.Visibility = Visibility.Collapsed;
            SearchBoxSecond.Visibility = Visibility.Visible;

            // Set focus to SearchBox
            SearchBoxSecond.Focus();

            MainViewModel.SelectedDialogInSearch =
                new DialogInSearchModel
                {
                    Id = MainViewModel.SelectedDialog.Id,
                    FirstName = MainViewModel.SelectedDialog.User.FirstName,
                    LastName = MainViewModel.SelectedDialog.User.LastName,
                    PhotoUrl = MainViewModel.SelectedDialog.User.PhotoUrl
                };
        }

        private void OnCloseModalClick(object sender, RoutedEventArgs e)
        {
            var animation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.1));

            animation.Completed += (_, _) =>
            {
                ModalWindow.IsOpen = false;

                SettingsManager.GoBack();
            };

            ModalWindow.BeginAnimation(OpacityProperty, animation);
        }

        private void OnCloseLanguageModalClick(object sender, RoutedEventArgs e)
        {
            var animation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.1));

            animation.Completed += (_, _) => { LanguageModalWindow.IsOpen = false; };

            LanguageModalWindow.BeginAnimation(OpacityProperty, animation);
        }

        private void Settings_Profile(object sender, MouseButtonEventArgs e)
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

        private void GoBack(object sender, RoutedEventArgs e)
        {
            SettingsManager.GoBack();
        }

        private void OnCloseLastActivityModalClick(object sender, RoutedEventArgs e)
        {
            var animation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.1));

            animation.Completed += (_, _) => { LastActivityModalWindow.IsOpen = false; };

            LastActivityModalWindow.BeginAnimation(OpacityProperty, animation);
        }

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

        private void OnSelectedChat(object sender, RoutedEventArgs e)
        {
            // var activeChatVisibilityNone = MainViewModel.ActiveChatVisibilityNone;
            //
            // if (!activeChatVisibilityNone) return;
            MainViewModel.ActiveChatVisibilityNone = false;
            MainViewModel.ActiveChatVisibility = true;

            if (MessagesListView.Items.IsEmpty) return;
            MainWindow_CheckMessageOnScreen();
            // If I have IsRead = false, then scroll to first unread message, else scroll to last message
            var firstUnreadMessage =
                MainViewModel.SelectedDialog.Messages.FirstOrDefault(
                    x => x.IsRead == false && x.Sender.Id != MainViewModel.MyProfile.Id
                );
            MessagesListView.ScrollIntoView(firstUnreadMessage ?? MainViewModel.SelectedDialog.Messages.Last());
        }

        private void OnSelectedUser(object sender, MouseButtonEventArgs e)
        {
            MainViewModel.ActiveChatVisibilityNone = false;
            MainViewModel.ActiveChatVisibility = true;

            // Open chat
            var dialog = MainViewModel.Dialogs.FirstOrDefault(x => x.User.Id == MainViewModel.SelectedUser.Id);

            if (dialog == null)
            {
                var newDialog = ControllerBase.DialogController.CreateDialog(MainViewModel.SelectedUser.Id);
                MainViewModel.Dialogs.Add(newDialog);
                MainViewModel.SelectedDialog = newDialog;
            }
            else
            {
                MainViewModel.SelectedDialog = dialog;
            }

            if (MessagesListView.Items.IsEmpty) return;
            MainWindow_CheckMessageOnScreen();
            var firstUnreadMessage =
                MainViewModel.SelectedDialog.Messages.FirstOrDefault(
                    x => x.IsRead == false && x.Sender.Id != MainViewModel.MyProfile.Id
                );
            MessagesListView.ScrollIntoView(firstUnreadMessage ?? MainViewModel.SelectedDialog.Messages.Last());
        }

        private void OnSelectedMessage(object sender, MouseButtonEventArgs e)
        {
            MainViewModel.ActiveChatVisibilityNone = false;
            MainViewModel.ActiveChatVisibility = true;

            // Open chat
            var dialog = MainViewModel.Dialogs.FirstOrDefault(x => x.User.Id == MainViewModel.SelectedDialog.User.Id);
            MainViewModel.SelectedDialog = dialog!;

            if (MessagesListView.Items.IsEmpty) return;
            MainWindow_CheckMessageOnScreen();
            if (MessagesListView.Items.Count > 0 && MessagesListView.SelectedIndex >= 0 &&
                MessagesListView.SelectedIndex < MessagesListView.Items.Count)
            {
                var thisMessage = MainViewModel.SelectedDialog.Messages[MessagesListView.SelectedIndex];
                MessagesListView.ScrollIntoView(thisMessage);
            }
            else
            {
                MessagesListView.ScrollIntoView(MainViewModel.SelectedDialog.Messages.Last());
            }
        }

        private void HandleScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            _mainWindow = this;
            if (MainViewModel.SelectedDialog == null!) return;
            MainWindow_CheckMessageOnScreen();
            MainViewModel.ScrollDownButtonVisibility = e.VerticalOffset < e.ExtentHeight - e.ViewportHeight - 320;
            if (e.VerticalOffset is > 48 or 0) return;
            var firstItem = MainWindow_GetFirstItemIsInViewport();
            MainViewModel.LoadMoreItems(firstItem);
            UpdateLayout();
        }

        private void ScrollDownButton_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
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

        public void CloseWindow()
        {
            Close();
            NotificationsManager.CloseAllNotifications();
            _notifyIconManager.DisposeNotifyIcon();
        }

        private CancellationTokenSource? _cancellationTokenSource;

        private async void SearchBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            // Delete old token
            _cancellationTokenSource?.Cancel();

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
                return;
            }
            if (e.Key == Key.Enter) MainViewModel.Search(SearchBox.Text);
            if (!IsTextKey(e.Key)) return;

            try
            {
                // Wait 1 second before search
                await Task.Delay(TimeSpan.FromSeconds(1), _cancellationTokenSource.Token);
                MainViewModel.Search(SearchBox.Text);
            }
            catch (TaskCanceledException)
            {
                // Ignore, if the task was canceled
            }
        }

        private async void SearchBoxSecond_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            // Delete old token
            _cancellationTokenSource.Cancel();

            // Create new token
            _cancellationTokenSource = new CancellationTokenSource();

            ClearSearchBoxButton.Visibility =
                string.IsNullOrWhiteSpace(SearchBoxSecond.Text) ? Visibility.Collapsed : Visibility.Visible;
            DialogsPanel.Visibility =
                string.IsNullOrWhiteSpace(SearchBoxSecond.Text) ? Visibility.Visible : Visibility.Collapsed;
            SearchInChatPanel.Visibility =
                string.IsNullOrWhiteSpace(SearchBoxSecond.Text) ? Visibility.Collapsed : Visibility.Visible;
            SearchAllPanel.Visibility = Visibility.Collapsed;

            if (string.IsNullOrWhiteSpace(SearchBoxSecond.Text))
            {
                MainViewModel.MessagesInSearch.Clear();
                return;
            }
            if (e.Key == Key.Enter) MainViewModel.SearchInDialog(SearchBoxSecond.Text);
            if (!IsTextKey(e.Key)) return;

            try
            {
                // Wait 1 second before search
                await Task.Delay(TimeSpan.FromSeconds(1), _cancellationTokenSource.Token);
                MainViewModel.SearchInDialog(SearchBoxSecond.Text);
            }
            catch (TaskCanceledException)
            {
                // Ignore, if the task was canceled
            }
        }

        private static bool IsTextKey(Key key)
        {
            // Check if the key is a letter or a digit
            return key is >= Key.A and <= Key.Z or >= Key.D0 and <= Key.D9 or >= Key.NumPad0 and <= Key.NumPad9
                or >= Key.Oem1 and <= Key.OemClear;
        }

        private void SelectFileInDialog(object sender, MouseButtonEventArgs e)
        {
            // Open file dialog
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg;*.gif)|*.png;*.jpeg;*.jpg;*.gif",
                Multiselect = false,
                Title = "Select a file"
            };
            
            if (openFileDialog.ShowDialog() != true) return;
            var fileName = openFileDialog.FileName;
            if (string.IsNullOrWhiteSpace(fileName)) return;
            var fileInfo = new FileInfo(fileName);
            var fileSize = fileInfo.Length;
            if (fileSize > 5242880)
            {
                MessageBox.Show("File size is too big. Max file size is 5MB", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            
            var fileExtension = Path.GetExtension(fileName);
            var fileExtensionLower = fileExtension.ToLower();
            if (fileExtensionLower != ".png" && fileExtensionLower != ".jpeg" && fileExtensionLower != ".jpg" && fileExtensionLower != ".gif")
            {
                MessageBox.Show("File type is not supported. Supported file types are: png, jpeg, jpg, gif", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            
            var fileBytes = File.ReadAllBytes(fileName);
            var fileBase64 = Convert.ToBase64String(fileBytes);
            var selectedDialog = MainViewModel.SelectedDialog;
            var fileModel = new SendMessageModel
            {
                type = "SEND_MESSAGE",
                dialogId = selectedDialog.Id,
                file = new FileModel {
                    name = Path.GetFileName(fileName),
                    size = fileSize,
                    type = fileExtensionLower,
                    data = fileBase64
                }
            };
            
            WebSocketClient.Send(JsonConvert.SerializeObject(fileModel));
        }
    }
}
