using System;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Animation;
using FlyMessenger.MVVM.ViewModels;
using FlyMessenger.UserControls;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
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

        public MainWindow()
        {
            InitializeComponent();
            _notifyIconManager.InitializeNotifyIcon();
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
            // TODO: Create settings method
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
                resources.Add(new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/Resources/Colors/Dark.xaml")
                });
                
                _light = false;
            }
            else
            {
                resources.Add(new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/Resources/Colors/Light.xaml")
                });
                
                _light = true;
            }
            _notifyIconManager.InitializeNotifyIcon();
        }

        private void ActiveChat_Search_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Set focus to searchbox
            SearchBox.Focus();
            
            // TODO: Implement search method
        }
    }
}
