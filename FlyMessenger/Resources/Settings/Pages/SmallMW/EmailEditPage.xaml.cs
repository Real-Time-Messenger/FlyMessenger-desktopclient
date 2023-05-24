using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using FlyMessenger.Controllers;

namespace FlyMessenger.Resources.Settings.Pages.SmallMW
{
    /// <summary>
    /// Interaction logic for EmailEditPage.xaml
    /// </summary>
    public partial class EmailEditPage
    {
        public EmailEditPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Visual effects for email edit label.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
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

        /// <summary>
        /// Handler for email edit cancel button click.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void OnEmailEditCancelClick(object sender, RoutedEventArgs e)
        {
            var closeAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.1));
            if (Application.Current.MainWindow is not MainWindow mainWindow) return;

            closeAnimation.Completed += (_, _) => { mainWindow.EmailEditModalWindow.IsOpen = false; };

            mainWindow.EmailEditModalWindow.BeginAnimation(OpacityProperty, closeAnimation);
        }

        /// <summary>
        /// Handler for email edit save button click.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private async void OnEmailEditSaveClick(object sender, RoutedEventArgs e)
        {
            var email = EmailEditTextBox.Text;

            EmailErrorLabel.Visibility =
                IsValidLength(email, 3, 50) ? Visibility.Collapsed : Visibility.Visible;
            if (!IsValidLength(email, 3, 50)) return;
            
            EmailCaseErrorLabel.Visibility =
                IsValidEmail(email) ? Visibility.Collapsed : Visibility.Visible;
            if (!IsValidEmail(email)) return;

            await ControllerBase.UserController.EditMyProfileEmail(email);

            EmailEditLabel.Opacity = 0.5;
            EmailEditTextBoxBorder.BorderThickness = new Thickness(0, 0, 0, 1);
        }

        /// <summary>
        /// Validate email length.
        /// </summary>
        /// <param name="text">Text to validate.</param>
        /// <param name="min">Min length.</param>
        /// <param name="max">Max length.</param>
        /// <returns>True if valid, false otherwise.</returns>
        private static bool IsValidLength(string text, int min, int max)
        {
            return text.Length >= min && text.Length <= max;
        }

        /// <summary>
        /// Validate email case.
        /// </summary>
        /// <param name="email">Email to validate.</param>
        /// <returns>True if valid, false otherwise.</returns>
        private static bool IsValidEmail(string email)
        {
            return email.Split("@").Length == 2 && email.Split("@")[1].Split(".").Length > 1;
        }
    }
}
