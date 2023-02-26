using System;
using System.Net;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using FlyMessenger.Controllers;
using FlyMessenger.Resources.Languages;

namespace FlyMessenger.Resources.Authorization
{
    public partial class RegistrationPage
    {
        private bool _isPasswordHidden = true;
        public string? RegistrationErrorText { get; set; }

        public RegistrationPage()
        {
            InitializeComponent();

            Loaded += RegistrationPage_Loaded;
        }

        private void RegistrationPage_Loaded(object sender, RoutedEventArgs e)
        {
            PasswordTextBox.PasswordChar = '•';
            PasswordTextBox.Background = (VisualBrush)PasswordTextBox.Style.Resources["CueBannerBrush"];
            PasswordRepeatTextBox.PasswordChar = '•';
            PasswordRepeatTextBox.Background = (VisualBrush)PasswordRepeatTextBox.Style.Resources["CueBannerBrush"];
        }

        private void UsernameSetActive(object sender, MouseButtonEventArgs e)
        {
            SetActive(UsernameLabel, UsernameTextBoxBorder);
        }

        private void EmailSetActive(object sender, MouseButtonEventArgs e)
        {
            SetActive(EmailLabel, EmailTextBoxBorder);
        }

        private void PasswordSetActive(object sender, MouseButtonEventArgs e)
        {
            SetActive(PasswordLabel, PasswordTextBoxBorder);
        }

        private void PasswordRepeatSetActive(object sender, MouseButtonEventArgs e)
        {
            SetActive(PasswordRepeatLabel, PasswordRepeatTextBoxBorder);
        }

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

        private void PasswordHandler(object sender, KeyEventArgs e)
        {
            PasswordTextBox.Background = PasswordTextBox.Password.Length == 0
                ? (VisualBrush)PasswordTextBox.Style.Resources["CueBannerBrush"]
                : Brushes.Transparent;
            ShowPassword.Text = PasswordTextBox.Password;
        }

        private void PasswordHandover(object sender, KeyEventArgs e)
        {
            PasswordTextBox.Password = ShowPassword.Text;
        }

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

        private void PasswordRepeatHandler(object sender, KeyEventArgs e)
        {
            PasswordRepeatTextBox.Background = PasswordRepeatTextBox.Password.Length == 0
                ? (VisualBrush)PasswordRepeatTextBox.Style.Resources["CueBannerBrush"]
                : Brushes.Transparent;
            ShowRepeatPassword.Text = PasswordRepeatTextBox.Password;
        }

        private void PasswordRepeatHandover(object sender, KeyEventArgs e)
        {
            PasswordRepeatTextBox.Password = ShowRepeatPassword.Text;
        }

        private async void RegisterButtonClick(object sender, MouseButtonEventArgs e)
        {
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

                switch (result.StatusCode)
                {
                    case HttpStatusCode.OK:
                        ShowRegisterButton(false);
                        RegisterErrorTextBlock.Visibility = Visibility.Collapsed;
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

        private void ShowRegisterButton(bool show)
        {
            RegisterButton.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            RegisterButtonLoading.Visibility = show ? Visibility.Collapsed : Visibility.Visible;
        }

        private static bool IsValidLength(string text, int min, int max)
        {
            return text.Length >= min && text.Length <= max;
        }

        private static bool IsValidEmail(string email)
        {
            return email.Split("@").Length == 2 && email.Split("@")[1].Split(".").Length > 1;
        }

        private void LoginButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (Window.GetWindow(this) is LoginWindow loginWindow)
            {
                loginWindow.Frame.Content = new LoginPage();
            }
        }
    }
}
