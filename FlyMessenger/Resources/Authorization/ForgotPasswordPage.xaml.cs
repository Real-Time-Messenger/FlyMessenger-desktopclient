using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using FlyMessenger.Controllers;
using FlyMessenger.Resources.Languages;

namespace FlyMessenger.Resources.Authorization
{
    /// <summary>
    /// Interaction logic for ForgotPasswordPage.xaml
    /// </summary>
    public partial class ForgotPasswordPage
    {
        public ForgotPasswordPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Visual active effect for email label.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void EmailSetActive(object sender, MouseButtonEventArgs e)
        {
            // Set animation
            var animation = new DoubleAnimation(EmailLabel.Opacity, 1, TimeSpan.FromSeconds(0.1));
            var animationBorder = new ThicknessAnimation(
                new Thickness(0, 0, 0, EmailTextBoxBorder.BorderThickness.Bottom),
                new Thickness(0, 0, 0, 2),
                TimeSpan.FromSeconds(0.1)
            );

            // Start animation
            EmailLabel.BeginAnimation(OpacityProperty, animation);
            EmailTextBoxBorder.BeginAnimation(Control.BorderThicknessProperty, animationBorder);
        }

        /// <summary>
        /// Button send click event handler.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private async void SendButtonClick(object sender, MouseButtonEventArgs e)
        {
            ShowSendButton(false);
            
            // Check email errors
            EmailErrorLabel.Visibility =
                IsValidLength(EmailTextBox.Text, 3, 254) ? Visibility.Collapsed : Visibility.Visible;
            if (EmailErrorLabel.Visibility == Visibility.Collapsed)
            {
                EmailCaseErrorLabel.Visibility = IsValidEmail(EmailTextBox.Text) ? Visibility.Collapsed : Visibility.Visible;
            }
            else
            {
                EmailCaseErrorLabel.Visibility = Visibility.Collapsed;
            }
            
            // If all data is valid
            if (EmailErrorLabel.Visibility == Visibility.Collapsed &&
                EmailCaseErrorLabel.Visibility == Visibility.Collapsed)
            {
                // Call reset password
                var result = await ControllerBase.UserController.CallResetPassword(
                    EmailTextBox.Text
                );

                // Check result
                switch (result.StatusCode)
                {
                    case HttpStatusCode.OK:
                        ShowSendButton(false);
                        SendErrorTextBlock.Visibility = Visibility.Collapsed;
                        if (result.Data is null)
                        {
                            // Show error
                            SendErrorTextBlock.Visibility = Visibility.Visible;
                            SendErrorTextBlock.Text = App.LanguageUtils.GetTranslation("CHECK_EMAIL");
                            SendErrorTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(69, 202, 36));
                            SendErrorTextBlock.Opacity = 0.85;
                            ShowSendButton(true);
                        }
                        break;
                    default:
                        ShowSendButton(true);

                        // Show possible errors
                        if (result.Data?.Details != null)
                        {
                            foreach (var detail in result.Data.Details)
                            {
                                if (detail.Field != "email") continue;
                                EmailErrorLabel.Visibility = Visibility.Visible;
                                EmailErrorLabel.Text = detail.Translation != null
                                    ? App.LanguageUtils.GetTranslation(detail.Translation!)
                                    : lang.emailIsNotValid;
                            }
                        }
                        if (result.Data?.Translation != null)
                        {
                            SendErrorTextBlock.Visibility = Visibility.Visible;
                            SendErrorTextBlock.Text = App.LanguageUtils.GetTranslation(result.Data.Translation);
                        }
                        break;
                }
            }
            else
            {
                ShowSendButton(true);
            }
        }

        /// <summary>
        /// Email validation.
        /// </summary>
        /// <param name="email">The email to validate.</param>
        /// <returns>True if email is valid, false otherwise.</returns>
        private static bool IsValidEmail(string email)
        {
            return email.Split("@").Length == 2 && email.Split("@")[1].Split(".").Length > 1;
        }

        /// <summary>
        /// Length validation.
        /// </summary>
        /// <param name="text">The text to validate.</param>
        /// <param name="min">Minimum length.</param>
        /// <param name="max">Maximum length.</param>
        /// <returns>True if length is valid, false otherwise.</returns>
        private static bool IsValidLength(string text, int min, int max)
        {
            return text.Length >= min && text.Length <= max;
        }

        /// <summary>
        /// Show or hide send button.
        /// </summary>
        /// <param name="show">True to show, false to hide.</param>
        private void ShowSendButton(bool show)
        {
            SendButton.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            SendButtonLoading.Visibility = show ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Login button click event handler.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void LoginButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (Window.GetWindow(this) is LoginWindow loginWindow)
            {
                loginWindow.Frame.Content = new LoginPage();
            }
        }

        /// <summary>
        /// Register button click event handler.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void RegisterButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (Window.GetWindow(this) is LoginWindow loginWindow)
            {
                loginWindow.Frame.Content = new RegistrationPage();
            }
        }
    }
}
