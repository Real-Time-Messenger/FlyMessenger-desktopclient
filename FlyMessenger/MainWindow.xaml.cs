using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Animation;
using FlyMessenger.Core;
using FlyMessenger.Core.Models;
using FlyMessenger.Core.Utils;
using FlyMessenger.HTTP;
using FlyMessenger.MVVM.ViewModels;
using FlyMessenger.Resources.Settings;
using Microsoft.VisualBasic.ApplicationServices;
using Newtonsoft.Json;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace FlyMessenger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // NotifyIconManager handler
        private readonly NotifyIconManager _notifyIconManager = new NotifyIconManager();
        private readonly WebSockets _webSocketClient = new WebSockets();
        private bool _light = true;
        public string LangSwitch { get; set; } = "";

        public MainWindow()
        {
            var langCode = Properties.Settings.Default.LanguageCode;
            Thread.CurrentThread.CurrentUICulture = langCode != "" ? new CultureInfo(langCode) : new CultureInfo("");

            InitializeComponent();
            PreviewKeyDown += MainWindowPreviewKeyDown;

            Closed += App.ToggleLanguage;
            Loaded += MainWindow_Loaded;

            DataContext = new MainViewModel();
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext).ActiveChatVisibilityNone = true;
            ((MainViewModel)DataContext).ActiveChatVisibility = false;
            
            await _webSocketClient.ConnectAsync(new Uri("ws://localhost:8000/ws?token=" + HttpClientBase.GetToken()));

            var result = _webSocketClient.ReceiveAsync();

            MessageBox.Show(result.Result);
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

        // Mouse up event
        private void UIElement_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            Properties.Settings.Default.LanguageCode = "ru-RU";
            Properties.Settings.Default.Save();
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
            var borderVisibility = ((MainViewModel)DataContext).BorderVisibility;

            if (borderVisibility) return;
            var animation = new DoubleAnimation(
                DarkBorder.Opacity,
                0.25,
                TimeSpan.FromSeconds(0.2)
            );

            ((MainViewModel)DataContext).BorderVisibility = true;

            DarkBorder.BeginAnimation(OpacityProperty, animation);
        }

        private void SideBar_OnMouseLeave(object sender, MouseEventArgs e)
        {
            var borderVisibility = ((MainViewModel)DataContext).BorderVisibility;

            if (!borderVisibility) return;
            var animation = new DoubleAnimation(
                DarkBorder.Opacity,
                0,
                TimeSpan.FromSeconds(0.2)
            );

            animation.Completed += (o, args) => ((MainViewModel)DataContext).BorderVisibility = false;

            DarkBorder.BeginAnimation(OpacityProperty, animation);
        }

        // Mouse left button up event to change IsActive property
        private void LogoutChatMenuButton_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // TODO: Create logout method
        }

        private void ChatChatMenuButton_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // TODO: Create chat method
        }

        private void ConservationsChatMenuButton_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // TODO: Create conservations method
        }

        private void GroupsChatMenuButton_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // TODO: Create groups method
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
            // TODO: Create support method
        }

        private void ThemeChatMenuButton_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var resources = Application.Current.Resources.MergedDictionaries;

            // Clear all resources
            resources.Clear();

            // Add new resources
            if (_light)
            {
                resources.Add(
                    new ResourceDictionary
                    {
                        Source = new Uri("pack://application:,,,/Resources/Colors/Dark.xaml")
                    }
                );

                _light = false;
            }
            else
            {
                resources.Add(
                    new ResourceDictionary
                    {
                        Source = new Uri("pack://application:,,,/Resources/Colors/Light.xaml")
                    }
                );

                _light = true;
            }
        }

        private void ActiveChat_Search_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Set focus to SearchBox
            SearchBox.Focus();

            // TODO: Implement search method
        }

        private void OnCloseModalClick(object sender, RoutedEventArgs e)
        {
            var animation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.1));

            animation.Completed += (o, args) =>
            {
                ModalWindow.IsOpen = false;

                SettingsManager.GoBack();
            };

            ModalWindow.BeginAnimation(OpacityProperty, animation);
        }

        private void OnCloseLanguageModalClick(object sender, RoutedEventArgs e)
        {
            var animation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.1));

            animation.Completed += (o, args) => { LanguageModalWindow.IsOpen = false; };

            LanguageModalWindow.BeginAnimation(OpacityProperty, animation);
        }

        private void Settings_Profile(object sender, MouseButtonEventArgs e)
        {
            // // Create animation that will be used to show modal window with opacity and slide from right to left
            // var animation = new DoubleAnimation(
            //     0,
            //     1,
            //     TimeSpan.FromSeconds(0.1)
            // );
            // var animationExit = new DoubleAnimation(
            //     1,
            //     0,
            //     TimeSpan.FromSeconds(0.1)
            // );
            // var slideAnimation = new ThicknessAnimation(
            //     new Thickness(0, 0, -25, 0),
            //     new Thickness(0, 0, 0, 0),
            //     TimeSpan.FromSeconds(0.1)
            // );
            // var slideAnimationExit = new ThicknessAnimation(
            //     new Thickness(0, 0, 0, 0),
            //     new Thickness(0, 0, -25, 0),
            //     TimeSpan.FromSeconds(0.1)
            // );
            //
            // animationExit.Completed += (o, args) =>
            // {
            //     ((MainViewModel)DataContext).IsModalWindowMainVisible = false;
            //     ((MainViewModel)DataContext).IsModalWindowProfileVisible = true;
            // };
            //
            // // Start animation
            // MainModalWindow.BeginAnimation(OpacityProperty, animationExit);
            // MainModalWindow.BeginAnimation(MarginProperty, slideAnimationExit);
            // ModalWindowProfile.BeginAnimation(OpacityProperty, animation);
            // ModalWindowProfile.BeginAnimation(MarginProperty, slideAnimation);
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            SettingsManager.GoBack();
        }

        private void OnCloseLastActivityModalClick(object sender, RoutedEventArgs e)
        {
            var animation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.1));

            animation.Completed += (o, args) => { LastActivityModalWindow.IsOpen = false; };

            LastActivityModalWindow.BeginAnimation(OpacityProperty, animation);
        }

        private async void OnReleaseKeyUp(object? sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            var text = ActiveChatMessage.Text;
            ActiveChatMessage.Text = string.Empty;

            if (string.IsNullOrEmpty(text)) return;

            var message = new SendMessageModel
            {
                type = "SEND_MESSAGE",
                file = null,
                dialogId = ((MainViewModel)DataContext).SelectedDialog.Id,
                text = text
            };
            
            for (var i = 0; i < 10000; i++)
            {
                await _webSocketClient.SendAsync(JsonConvert.SerializeObject(message));
            }
            // await _webSocketClient.SendAsync(JsonConvert.SerializeObject(message));
        }

        private async void OnPressKeyDown(object sender, KeyEventArgs e)
        {
            var message = new SendMessageModel
            {
                type = "TYPING",
                dialogId = ((MainViewModel)DataContext).SelectedDialog.Id,
            };
            
            await _webSocketClient.SendAsync(JsonConvert.SerializeObject(message));
        }

        private void OnSelectedChat(object sender, RoutedEventArgs e)
        {
            var activeChatVisibilityNone = ((MainViewModel)DataContext).ActiveChatVisibilityNone;

            if (!activeChatVisibilityNone) return;
            ((MainViewModel)DataContext).ActiveChatVisibilityNone = false;
            ((MainViewModel)DataContext).ActiveChatVisibility = true;
        }
    }
}
