using System;
using System.Net;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using FlyMessenger.Controllers;
using FlyMessenger.HTTP;
using FlyMessenger.Resources.Languages;

namespace FlyMessenger.Resources.Authorization
{
    public partial class LoginPage
    {
        private bool _isPasswordHidden = true;
        public string? LoginErrorText { get; set; }

        public LoginPage()
        {
            InitializeComponent();

            Loaded += LoginPage_Loaded;
        }

        private void LoginPage_Loaded(object sender, RoutedEventArgs e)
        {
            PasswordTextBox.PasswordChar = '•';
            PasswordTextBox.Background = (VisualBrush)PasswordTextBox.Style.Resources["CueBannerBrush"];
        }

        private void UsernameSetActive(object sender, MouseButtonEventArgs e)
        {
            // Set animation
            var animation = new DoubleAnimation(UsernameLabel.Opacity, 1, TimeSpan.FromSeconds(0.1));
            var animationBorder = new ThicknessAnimation(
                new Thickness(0, 0, 0, UsernameTextBoxBorder.BorderThickness.Bottom),
                new Thickness(0, 0, 0, 2),
                TimeSpan.FromSeconds(0.1)
            );
            var animationExit = new DoubleAnimation(PasswordLabel.Opacity, 0.5, TimeSpan.FromSeconds(0.1));
            var animationBorderExit = new ThicknessAnimation(
                new Thickness(0, 0, 0, PasswordTextBoxBorder.BorderThickness.Bottom),
                new Thickness(0, 0, 0, 1),
                TimeSpan.FromSeconds(0.1)
            );

            // Start animation
            UsernameLabel.BeginAnimation(OpacityProperty, animation);
            UsernameTextBoxBorder.BeginAnimation(Control.BorderThicknessProperty, animationBorder);
            PasswordLabel.BeginAnimation(OpacityProperty, animationExit);
            PasswordTextBoxBorder.BeginAnimation(Control.BorderThicknessProperty, animationBorderExit);
        }

        private void PasswordSetActive(object sender, MouseButtonEventArgs e)
        {
            // Set animation
            var animation = new DoubleAnimation(PasswordLabel.Opacity, 1, TimeSpan.FromSeconds(0.1));
            var animationBorder = new ThicknessAnimation(
                new Thickness(0, 0, 0, PasswordTextBoxBorder.BorderThickness.Bottom),
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
            PasswordLabel.BeginAnimation(OpacityProperty, animation);
            PasswordTextBoxBorder.BeginAnimation(Control.BorderThicknessProperty, animationBorder);
            UsernameLabel.BeginAnimation(OpacityProperty, animationExit);
            UsernameTextBoxBorder.BeginAnimation(Control.BorderThicknessProperty, animationBorderExit);
        }

        private void PasswordEyeClick(object sender, MouseButtonEventArgs e)
        {
            _isPasswordHidden = !_isPasswordHidden;
            PasswordTextBox.Visibility = _isPasswordHidden ? Visibility.Visible : Visibility.Collapsed;
            ShowPassword.Visibility = _isPasswordHidden ? Visibility.Collapsed : Visibility.Visible;
            PasswordEyeImage.SetResourceReference(
                Image.SourceProperty,
                _isPasswordHidden ? "PasswordEyeIcon-Opened" : "PasswordEyeIcon-Closed"
            );
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

        private async void LoginButtonClick(object sender, MouseButtonEventArgs e)
        {
            ShowLoginButton(false);
            UsernameErrorLabel.Visibility =
                IsValidLength(UsernameTextBox.Text, 3, 50) ? Visibility.Collapsed : Visibility.Visible;
            PasswordErrorLabel.Visibility =
                IsValidLength(PasswordTextBox.Password, 8, 32) ? Visibility.Collapsed : Visibility.Visible;

            if (UsernameErrorLabel.Visibility == Visibility.Collapsed && PasswordErrorLabel.Visibility == Visibility.Collapsed)
            {
                var result = await ControllerBase.UserController.Login(UsernameTextBox.Text, PasswordTextBox.Password);

                if (result.Data == null) return;
                switch (result.StatusCode)
                {
                    case HttpStatusCode.OK:
                        ShowLoginButton(false);
                        LoginErrorTextBlock.Visibility = Visibility.Collapsed;
                        if (result.Data.Token != null)
                        {
                            HttpClientBase.SetToken(result.Data.Token);
                            if (Window.GetWindow(this) is LoginWindow loginWindow)
                            {
                                Application.Current.MainWindow = new MainWindow();
                                Application.Current.MainWindow.Show();
                                Application.Current.MainWindow.Activate();
                                loginWindow.Close();
                            }
                        }
                        
                        if (result.Data.Event != null)
                            switch (result.Data.Event)
                            {
                                case "TWO_FACTOR":
                                {
                                    if (Window.GetWindow(this) is LoginWindow loginWindow)
                                    {
                                        loginWindow.Frame.Content = new TwoFactorPage();
                                    }
                                    break;
                                }
                                case "NEW_DEVICE":
                                {
                                    if (Window.GetWindow(this) is LoginWindow loginWindow)
                                    {
                                        loginWindow.Frame.Content = new NewDevicePage();
                                    }
                                    break;
                                }
                            }
                        break;
                    default:
                        ShowLoginButton(true);
                        LoginErrorTextBlock.Visibility = Visibility.Visible;
                        LoginErrorText = result.Data.Translation;
                        if (LoginErrorText != null)
                        {
                            LoginErrorTextBlock.Text = App.LanguageUtils.GetTranslation(LoginErrorText);
                        }
                        break;
                }
            }
            else
            {
                ShowLoginButton(true);
            }
        }

        private void ShowLoginButton(bool show)
        {
            LoginButtonLoading.Visibility = show ? Visibility.Collapsed : Visibility.Visible;
            LoginButton.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }

        private static bool IsValidLength(string str, int minLength, int maxLength)
        {
            return str.Length >= minLength && str.Length <= maxLength;
        }

        private void RegisterButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (Window.GetWindow(this) is not LoginWindow loginWindow) return;
            loginWindow.Frame.Content = new RegistrationPage();
        }

        private void ForgotPasswordButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (Window.GetWindow(this) is not LoginWindow loginWindow) return;
            loginWindow.Frame.Content = new ForgotPasswordPage();
        }
    }
}
