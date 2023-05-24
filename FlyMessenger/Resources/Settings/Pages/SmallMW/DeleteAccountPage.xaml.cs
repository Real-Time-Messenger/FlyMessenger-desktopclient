using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using FlyMessenger.Controllers;
using FlyMessenger.Core.Utils;

namespace FlyMessenger.Resources.Settings.Pages.SmallMW
{
    /// <summary>
    /// Interaction logic for DeleteAccountPage.xaml
    /// </summary>
    public partial class DeleteAccountPage : Page
    {
        public DeleteAccountPage()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// Handler for click on cancel button.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
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

        /// <summary>
        /// Handler for click on delete button.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private async void OnDeleteAccountClick(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is not MainWindow mainWindow) return;
            await ControllerBase.UserController.Delete();
            var tokenSettings = new TokenSettings();
            tokenSettings.Delete();
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            mainWindow.Close();
        }
    }
}

