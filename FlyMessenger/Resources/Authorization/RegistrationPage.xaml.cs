using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using FlyMessenger.Controllers;

namespace FlyMessenger.Resources.Authorization
{
    /// <summary>
    /// Interaction logic for RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage
    {
        private bool _isPasswordHidden = true;

        public RegistrationPage()
        {
            InitializeComponent();

            Loaded += RegistrationPage_Loaded;
        }

        /// <summary>
        /// Set password hidden with char '•'.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void RegistrationPage_Loaded(object sender, RoutedEventArgs e)
        {
            PasswordTextBox.PasswordChar = '•';
            PasswordTextBox.Background = (VisualBrush)PasswordTextBox.Style.Resources["CueBannerBrush"];
            PasswordRepeatTextBox.PasswordChar = '•';
            PasswordRepeatTextBox.Background = (VisualBrush)PasswordRepeatTextBox.Style.Resources["CueBannerBrush"];
        }

        /// <summary>
        /// Visual active effect for username label.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void UsernameSetActive(object sender, MouseButtonEventArgs e)
        {
            SetActive(UsernameLabel, UsernameTextBoxBorder);
        }

        /// <summary>
        /// Visual active effect for email label.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void EmailSetActive(object sender, MouseButtonEventArgs e)
        {
            SetActive(EmailLabel, EmailTextBoxBorder);
        }

        /// <summary>
        /// Visual active effect for password label.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void PasswordSetActive(object sender, MouseButtonEventArgs e)
        {
            SetActive(PasswordLabel, PasswordTextBoxBorder);
        }

        /// <summary>
        /// Visual active effect for password repeat label.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void PasswordRepeatSetActive(object sender, MouseButtonEventArgs e)
        {
            SetActive(PasswordRepeatLabel, PasswordRepeatTextBoxBorder);
        }

        /// <summary>
        /// Visual active effect for label and border.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="border">The border.</param>
        private void SetActive(UIElement label, Border border)
        {
            // Set animation
            var animation = new DoubleAnimation(label.Opacity, 1, TimeSpan.FromSeconds(0.1));
            var animationBorder = new ThicknessAnimation(
                new Thickness(0, 0, 0, border.BorderThickness.Bottom),
                new Thickness(0, 0, 0, 2),
                TimeSpan.FromSeconds(0.1)
            );
            var animationExit = new DoubleAnimation(UsernameLabel.Opacity, 0.5, TimeSpan.FromSeconds(0.1));
            var animationBorderExit = new ThicknessAnimation(
                new Thickness(0, 0, 0, UsernameTextBoxBorder.BorderThickness.Bottom),
                new Thickness(0, 0, 0, 1),
                TimeSpan.FromSeconds(0.1)
            );

            // Start animation
            label.BeginAnimation(OpacityProperty, animation);
            border.BeginAnimation(Border.BorderThicknessProperty, animationBorder);

            // Exit animation
            foreach (var element in new[] { UsernameLabel, EmailLabel, PasswordLabel, PasswordRepeatLabel })
            {
                if (element == label) continue;
                if (Math.Abs(element.Opacity - 0.5) < 0.01) continue;
                element.BeginAnimation(OpacityProperty, animationExit);
            }
            foreach (var element in new[]
                         { UsernameTextBoxBorder, EmailTextBoxBorder, PasswordTextBoxBorder, PasswordRepeatTextBoxBorder })
            {
                if (element == border) continue;
                if (Math.Abs(element.BorderThickness.Bottom - 2) > 0.01) continue;
                element.BeginAnimation(Border.BorderThicknessProperty, animationBorderExit);
            }
        }

        /// <summary>
        /// Handler for password input.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void PasswordHandler(object sender, KeyEventArgs e)
        {
            PasswordTextBox.Background = PasswordTextBox.Password.Length == 0
                ? (VisualBrush)PasswordTextBox.Style.Resources["CueBannerBrush"]
                : Brushes.Transparent;
            ShowPassword.Text = PasswordTextBox.Password;
        }

        /// <summary>
        /// Handover password to ShowPassword label.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void PasswordHandover(object sender, KeyEventArgs e)
        {
            PasswordTextBox.Password = ShowPassword.Text;
        }

        /// <summary>
        /// Method for show/hide password.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void PasswordEyeClick(object sender, MouseButtonEventArgs e)
        {
            _isPasswordHidden = !_isPasswordHidden;

            ShowPassword.Visibility = _isPasswordHidden ? Visibility.Collapsed : Visibility.Visible;
            PasswordTextBox.Visibility = _isPasswordHidden ? Visibility.Visible : Visibility.Collapsed;
            ShowRepeatPassword.Visibility = _isPasswordHidden ? Visibility.Collapsed : Visibility.Visible;
            PasswordRepeatTextBox.Visibility = _isPasswordHidden ? Visibility.Visible : Visibility.Collapsed;

            PasswordEyeImage.SetResourceReference(
                Image.SourceProperty,
                _isPasswordHidden ? "PasswordEyeIcon-Opened" : "PasswordEyeIcon-Closed"
            );
        }

        /// <summary>
        /// Handler for password repeat input.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void PasswordRepeatHandler(object sender, KeyEventArgs e)
        {
            PasswordRepeatTextBox.Background = PasswordRepeatTextBox.Password.Length == 0
                ? (VisualBrush)PasswordRepeatTextBox.Style.Resources["CueBannerBrush"]
                : Brushes.Transparent;
            ShowRepeatPassword.Text = PasswordRepeatTextBox.Password;
        }

        /// <summary>
        /// Handover password repeat to ShowRepeatPassword label.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void PasswordRepeatHandover(object sender, KeyEventArgs e)
        {
            PasswordRepeatTextBox.Password = ShowRepeatPassword.Text;
        }

        /// <summary>
        /// Register button click handler.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private async void RegisterButtonClick(object sender, MouseButtonEventArgs e)
        {
            // Check if all fields are valid
            ShowRegisterButton(false);
            UsernameErrorLabel.Visibility =
                IsValidLength(UsernameTextBox.Text, 3, 50) ? Visibility.Collapsed : Visibility.Visible;
            PasswordErrorLabel.Visibility =
                IsValidLength(PasswordTextBox.Password, 8, 32) ? Visibility.Collapsed : Visibility.Visible;
            EmailErrorLabel.Visibility =
                IsValidLength(EmailTextBox.Text, 3, 254) ? Visibility.Collapsed : Visibility.Visible;
            PasswordRepeatErrorLabel.Visibility =
                IsValidLength(PasswordRepeatTextBox.Password, 8, 32) ? Visibility.Collapsed : Visibility.Visible;
            if (EmailErrorLabel.Visibility == Visibility.Collapsed)
            {
                EmailCaseErrorLabel.Visibility = IsValidEmail(EmailTextBox.Text) ? Visibility.Collapsed : Visibility.Visible;
            }
            else
            {
                EmailCaseErrorLabel.Visibility = Visibility.Collapsed;
            }
            PasswordRepeatCaseErrorLabel.Visibility =
                PasswordTextBox.Password == PasswordRepeatTextBox.Password ? Visibility.Collapsed : Visibility.Visible;
            
            if (UsernameErrorLabel.Visibility == Visibility.Collapsed && PasswordErrorLabel.Visibility == Visibility.Collapsed &&
                EmailErrorLabel.Visibility == Visibility.Collapsed &&
                PasswordRepeatErrorLabel.Visibility == Visibility.Collapsed &&
                EmailCaseErrorLabel.Visibility == Visibility.Collapsed &&
                PasswordRepeatCaseErrorLabel.Visibility == Visibility.Collapsed)
            {
                var result = await ControllerBase.UserController.Register(
                    UsernameTextBox.Text,
                    EmailTextBox.Text,
                    PasswordTextBox.Password,
                    PasswordRepeatTextBox.Password
                );

                // Status code handling
                switch (result.StatusCode)
                {
                    case HttpStatusCode.OK:
                        ShowRegisterButton(false);
                        RegisterErrorTextBlock.Visibility = Visibility.Collapsed;
                        
                        // Show activation required message
                        if (result.Data.Event is "ACTIVATION_REQUIRED")
                        {
                            RegisterErrorTextBlock.Visibility = Visibility.Visible;
                            RegisterErrorTextBlock.Text = App.LanguageUtils.GetTranslation("ACTIVATION_REQUIRED");
                            RegisterErrorTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(69, 202, 36));
                            RegisterErrorTextBlock.Opacity = 0.85;
                            ShowRegisterButton(true);
                        }
                        break;
                    default:
                        ShowRegisterButton(true);

                        foreach (var detail in result.Data.Details)
                        {
                            switch (detail.Field)
                            {
                                case "username":
                                {
                                    UsernameErrorLabel.Visibility = Visibility.Visible;
                                    if (detail.Translation != null)
                                        UsernameErrorLabel.Text = App.LanguageUtils.GetTranslation(detail.Translation);
                                    break;
                                }
                                case "email":
                                {
                                    EmailErrorLabel.Visibility = Visibility.Visible;
                                    if (detail.Translation != null)
                                        EmailErrorLabel.Text = App.LanguageUtils.GetTranslation(detail.Translation);
                                    break;
                                }
                                case "password":
                                {
                                    PasswordErrorLabel.Visibility = Visibility.Visible;
                                    if (detail.Translation != null)
                                        PasswordErrorLabel.Text = App.LanguageUtils.GetTranslation(detail.Translation);
                                    break;
                                }
                                case "passwordConfirm":
                                {
                                    PasswordRepeatErrorLabel.Visibility = Visibility.Visible;
                                    if (detail.Translation != null)
                                        PasswordRepeatErrorLabel.Text = App.LanguageUtils.GetTranslation(detail.Translation);
                                    break;
                                }
                            }
                        }
                        break;
                }
            }
            else
            {
                ShowRegisterButton(true);
            }
        }

        /// <summary>
        /// Show or hide register button.
        /// </summary>
        /// <param name="show">Show or hide.</param>
        private void ShowRegisterButton(bool show)
        {
            RegisterButton.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            RegisterButtonLoading.Visibility = show ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Check if text is valid length.
        /// </summary>
        /// <param name="text">Text to check.</param>
        /// <param name="min">Minimum length.</param>
        /// <param name="max">Maximum length.</param>
        /// <returns>True if valid length.</returns>
        private static bool IsValidLength(string text, int min, int max)
        {
            return text.Length >= min && text.Length <= max;
        }

        /// <summary>
        /// Check if email is valid.
        /// </summary>
        /// <param name="email">Email to check.</param>
        /// <returns>True if valid email.</returns>
        private static bool IsValidEmail(string email)
        {
            return email.Split("@").Length == 2 && email.Split("@")[1].Split(".").Length > 1;
        }

        /// <summary>
        /// Handler for login button click.
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
    }
}
