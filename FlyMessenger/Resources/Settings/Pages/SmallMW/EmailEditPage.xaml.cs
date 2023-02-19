using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using FlyMessenger.Controllers;

namespace FlyMessenger.Resources.Settings.Pages.SmallMW
{
    public partial class EmailEditPage
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
            if (Application.Current.MainWindow is not MainWindow mainWindow) return;
            
            closeAnimation.Completed += (_, _) =>
            {
                mainWindow.EmailEditModalWindow.IsOpen = false;
            };
            
            mainWindow.EmailEditModalWindow.BeginAnimation(OpacityProperty, closeAnimation);
        }

        private void OnEmailEditSaveClick(object sender, RoutedEventArgs e)
        {
            var email = EmailEditTextBox.Text;
            
            if (string.IsNullOrWhiteSpace(email))
            {
                var mainWindow = (MainWindow) Application.Current.MainWindow;
                if (mainWindow == null) return;
                if (mainWindow.CannotBeNullTip.IsOpen) return;

                mainWindow.CannotBeNullTip.IsOpen = true;

                var openAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.2));
                var closeAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.2))
                {
                    BeginTime = TimeSpan.FromSeconds(2)
                };

                openAnimation.Completed += (_, _) =>
                {
                    mainWindow.CannotBeNullTip.BeginAnimation(OpacityProperty, closeAnimation);
                    EmailEditTextBox.Text = ControllerBase.UserController.GetMyProfile().Email;
                };
                closeAnimation.Completed += (_, _) => mainWindow.CannotBeNullTip.IsOpen = false;

                mainWindow.CannotBeNullTip.BeginAnimation(OpacityProperty, openAnimation);
                return;
            }
            
            ControllerBase.UserController.EditMyProfileEmail(email);
            
            EmailEditLabel.Opacity = 0.5;
            EmailEditTextBoxBorder.BorderThickness = new Thickness(0, 0, 0, 1);
        }
    }
}

