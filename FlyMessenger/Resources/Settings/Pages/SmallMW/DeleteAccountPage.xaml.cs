using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using FlyMessenger.Controllers;
using FlyMessenger.Core.Utils;

namespace FlyMessenger.Resources.Settings.Pages.SmallMW
{
    public partial class DeleteAccountPage : Page
    {
        public DeleteAccountPage()
        {
            InitializeComponent();
        }
        
        private void OnDeleteCancelClick(object sender, RoutedEventArgs e)
        {
            var closeAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.1));
            if (Application.Current.MainWindow is not MainWindow mainWindow) return;
            closeAnimation.Completed += (o, args) =>
            {
                mainWindow.DeleteAccountModalWindow.IsOpen = false;
            };
            
            mainWindow.DeleteAccountModalWindow.BeginAnimation(OpacityProperty, closeAnimation);
        }

        private void OnDeleteAccountClick(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is not MainWindow mainWindow) return;
            ControllerBase.UserController.Delete();
            var tokenSettings = new TokenSettings();
            tokenSettings.Delete();
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            mainWindow.Close();
        }
    }
}

