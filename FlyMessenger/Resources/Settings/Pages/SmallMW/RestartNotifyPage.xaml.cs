using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace FlyMessenger.Resources.Settings.Pages.SmallMW
{
    public partial class RestartNotifyPage : Page
    {
        public RestartNotifyPage()
        {
            InitializeComponent();
        }

        private void OnRestartCancelClick(object sender, RoutedEventArgs e)
        {
            var window = (MainWindow)Application.Current.MainWindow!;
            var closeAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            
            closeAnimation.Completed += (s, _) => window.RestartNotifyModalWindow.IsOpen = false;
            
            window.RestartNotifyModalWindow.BeginAnimation(OpacityProperty, closeAnimation);
        }

        private void OnRestartClick(object sender, RoutedEventArgs e)
        {
            App.RestartApp();
        }
    }
}

