using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace FlyMessenger.Resources.Settings.Pages.SmallMW
{
    /// <summary>
    /// Interaction logic for RestartNotifyPage.xaml
    /// </summary>
    public partial class RestartNotifyPage : Page
    {
        public RestartNotifyPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handler for restart cancel button click.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void OnRestartCancelClick(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is not MainWindow window) return;
            var closeAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            
            closeAnimation.Completed += (s, _) => window.RestartNotifyModalWindow.IsOpen = false;
            
            window.RestartNotifyModalWindow.BeginAnimation(OpacityProperty, closeAnimation);
        }

        /// <summary>
        /// Handler for restart button click.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void OnRestartClick(object sender, RoutedEventArgs e)
        {
            App.RestartApp();
        }
    }
}

