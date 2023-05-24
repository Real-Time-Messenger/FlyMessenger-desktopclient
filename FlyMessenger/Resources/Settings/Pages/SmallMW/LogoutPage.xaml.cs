using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using FlyMessenger.Controllers;
using FlyMessenger.Core.Utils;

namespace FlyMessenger.Resources.Settings.Pages.SmallMW
{
    public partial class LogoutPage : Page
    {
        public LogoutPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handler for logout cancel button click.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void OnLogoutCancelClick(object sender, RoutedEventArgs e)
        {
            var closeAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.1));
            if (Application.Current.MainWindow is not MainWindow mainWindow) return;
            closeAnimation.Completed += (o, args) =>
            {
                mainWindow.LogoutModalWindow.IsOpen = false;
            };
            
            mainWindow.LogoutModalWindow.BeginAnimation(OpacityProperty, closeAnimation);
        }

        /// <summary>
        /// Handler for logout button click.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void OnLogoutClick(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is not MainWindow mainWindow) return;
            ControllerBase.UserController.Logout();
            var tokenSettings = new TokenSettings();
            tokenSettings.Delete();
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            mainWindow.Close();
        }
    }
}

