using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using FlyMessenger.Core.Utils;
using FlyMessenger.MVVM.ViewModels;
using FlyMessenger.Resources.Languages;
using FlyMessenger.Resources.Settings;
using FlyMessenger.UserControls;
using Application = System.Windows.Application;
using Clipboard = System.Windows.Clipboard;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
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
        private bool _light = true;
        public string LangSwitch { get; private set; } = "";

        public MainWindow()
        {
            var langCode = Properties.Settings.Default.LanguageCode;
            Thread.CurrentThread.CurrentUICulture = langCode != "" ? new CultureInfo(langCode) : new CultureInfo("");
            
            InitializeComponent();
            PreviewKeyDown += MainWindowPreviewKeyDown;

            Closed += App.ToggleLanguage;
            
            DataContext = new MainViewModel();
        }
        
        private void MainWindowPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
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
            _notifyIconManager.InitializeNotifyIcon();
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

        private void OnDeleteCancelClick(object sender, RoutedEventArgs e)
        {
            var closeAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.1));
            
            closeAnimation.Completed += (o, args) =>
            {
                DeleteAccountModalWindow.IsOpen = false;
            };
            
            DeleteAccountModalWindow.BeginAnimation(OpacityProperty, closeAnimation);
        }

        private void OnDeleteAccountClick(object sender, RoutedEventArgs e)
        {
            // Todo: Create delete account method
        }

        private void OnEstonianRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            LangSwitch = "et-EE";
            Close();
        }
        
        private void OnRussianRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            LangSwitch = "ru-RU";
            Close();
        }
        
        private void OnEnglishRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            LangSwitch = "en-US";
            Close();
        }
    }
}
