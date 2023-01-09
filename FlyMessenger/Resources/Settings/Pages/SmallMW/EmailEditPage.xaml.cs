using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using FlyMessenger.Controllers;

namespace FlyMessenger.Resources.Settings.Pages.SmallMW
{
    public partial class EmailEditPage : Page
    {
        public EmailEditPage()
        {
            InitializeComponent();
        }

        private void EmailSetActive(object sender, MouseButtonEventArgs e)
        {
            // Set animation
            var animation = new DoubleAnimation(EmailEditLabel.Opacity, 1, TimeSpan.FromSeconds(0.1));
            var animationBorder = new ThicknessAnimation(
                new Thickness(0, 0, 0, EmailEditTextBoxBorder.BorderThickness.Bottom),
                new Thickness(0, 0, 0, 2),
                TimeSpan.FromSeconds(0.1)
            );
            
            // Start animation
            EmailEditLabel.BeginAnimation(OpacityProperty, animation);
            EmailEditTextBoxBorder.BeginAnimation(Control.BorderThicknessProperty, animationBorder);
        }

        private void OnEmailEditCancelClick(object sender, RoutedEventArgs e)
        {
            var closeAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.1));
            var mainWindow = (MainWindow) Application.Current.MainWindow;
            
            closeAnimation.Completed += (o, args) =>
            {
                mainWindow.EmailEditModalWindow.IsOpen = false;
            };
            
            mainWindow.EmailEditModalWindow.BeginAnimation(OpacityProperty, closeAnimation);
        }

        private void OnEmailEditSaveClick(object sender, RoutedEventArgs e)
        {
            var email = EmailEditTextBox.Text;
            
            ControllerBase.UserController.EditMyProfileEmail(email);
            
            EmailEditLabel.Opacity = 0.5;
            EmailEditTextBoxBorder.BorderThickness = new Thickness(0, 0, 0, 1);
            
            App.RestartApp();
        }
    }
}

