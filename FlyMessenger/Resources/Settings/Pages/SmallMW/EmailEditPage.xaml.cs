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

            closeAnimation.Completed += (_, _) => { mainWindow.EmailEditModalWindow.IsOpen = false; };

            mainWindow.EmailEditModalWindow.BeginAnimation(OpacityProperty, closeAnimation);
        }

        private void OnEmailEditSaveClick(object sender, RoutedEventArgs e)
        {
            var email = EmailEditTextBox.Text;

            EmailErrorLabel.Visibility =
                IsValidLength(email, 3, 50) ? Visibility.Collapsed : Visibility.Visible;
            if (!IsValidLength(email, 3, 50)) return;
            
            EmailCaseErrorLabel.Visibility =
                IsValidEmail(email) ? Visibility.Collapsed : Visibility.Visible;
            if (!IsValidEmail(email)) return;

            ControllerBase.UserController.EditMyProfileEmail(email);

            EmailEditLabel.Opacity = 0.5;
            EmailEditTextBoxBorder.BorderThickness = new Thickness(0, 0, 0, 1);
        }

        private static bool IsValidLength(string text, int min, int max)
        {
            return text.Length >= min && text.Length <= max;
        }

        private static bool IsValidEmail(string email)
        {
            return email.Split("@").Length == 2 && email.Split("@")[1].Split(".").Length > 1;
        }
    }
}
