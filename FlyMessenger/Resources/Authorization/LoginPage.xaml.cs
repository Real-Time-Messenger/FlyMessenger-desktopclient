using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using FlyMessenger.Controllers;
using FlyMessenger.HTTP;

namespace FlyMessenger.Resources.Authorization
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage
    {
        private bool _isPasswordHidden = true;
        public string? LoginErrorText { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public LoginPage()
        {
            InitializeComponent();

            Loaded += LoginPage_Loaded;
        }

        /// <summary>
        /// Loaded event
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">The event data</param>
        private void LoginPage_Loaded(object sender, RoutedEventArgs e)
        {
            PasswordTextBox.PasswordChar = '•';
            PasswordTextBox.Background = (VisualBrush)PasswordTextBox.Style.Resources["CueBannerBrush"];
        }

        /// <summary>
        /// Visual active effect for username label.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UsernameSetActive(object sender, MouseButtonEventArgs e)
        {
            // Set animations
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

            // Start animations
            UsernameLabel.BeginAnimation(OpacityProperty, animation);
            UsernameTextBoxBorder.BeginAnimation(Control.BorderThicknessProperty, animationBorder);
            PasswordLabel.BeginAnimation(OpacityProperty, animationExit);
            PasswordTextBoxBorder.BeginAnimation(Control.BorderThicknessProperty, animationBorderExit);
        }

        /// <summary>
        /// Visual active effect for password label.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void PasswordSetActive(object sender, MouseButtonEventArgs e)
        {
            // Set animations
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

            // Start animations
            PasswordLabel.BeginAnimation(OpacityProperty, animation);
            PasswordTextBoxBorder.BeginAnimation(Control.BorderThicknessProperty, animationBorder);
            UsernameLabel.BeginAnimation(OpacityProperty, animationExit);
            UsernameTextBoxBorder.BeginAnimation(Control.BorderThicknessProperty, animationBorderExit);
        }

        /// <summary>
        /// Method for show password.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Handler for password input.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PasswordHandler(object sender, KeyEventArgs e)
        {
            PasswordTextBox.Background = PasswordTextBox.Password.Length == 0
                ? (VisualBrush)PasswordTextBox.Style.Resources["CueBannerBrush"]
                : Brushes.Transparent;
            ShowPassword.Text = PasswordTextBox.Password;
        }

        /// <summary>
        /// Method for password handover.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PasswordHandover(object sender, KeyEventArgs e)
        {
            PasswordTextBox.Password = ShowPassword.Text;
        }

        /// <summary>
        /// Login button click handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void LoginButtonClick(object sender, MouseButtonEventArgs e)
        {
            ShowLoginButton(false);
            
            // Check input data
            UsernameErrorLabel.Visibility =
                IsValidLength(UsernameTextBox.Text, 3, 50) ? Visibility.Collapsed : Visibility.Visible;
            PasswordErrorLabel.Visibility =
                IsValidLength(PasswordTextBox.Password, 8, 32) ? Visibility.Collapsed : Visibility.Visible;

            // Check if input data is valid
            if (UsernameErrorLabel.Visibility == Visibility.Collapsed && PasswordErrorLabel.Visibility == Visibility.Collapsed)
            {
                var result = await ControllerBase.UserController.Login(UsernameTextBox.Text, PasswordTextBox.Password);

                if (result.Data == null) return;
                
                // Status code handler
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

        /// <summary>
        /// Method for show or hide login button.
        /// </summary>
        /// <param name="show">Boolean for show or hide login button.</param>
        private void ShowLoginButton(bool show)
        {
            LoginButtonLoading.Visibility = show ? Visibility.Collapsed : Visibility.Visible;
            LoginButton.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Method for check string length.
        /// </summary>
        /// <param name="str">String for check.</param>
        /// <param name="minLength">Minimum length.</param>
        /// <param name="maxLength">Maximum length.</param>
        /// <returns>True if string length is valid, otherwise false.</returns>
        private static bool IsValidLength(string str, int minLength, int maxLength)
        {
            return str.Length >= minLength && str.Length <= maxLength;
        }

        /// <summary>
        /// Method for show registration page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegisterButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (Window.GetWindow(this) is not LoginWindow loginWindow) return;
            loginWindow.Frame.Content = new RegistrationPage();
        }

        /// <summary>
        /// Method for show forgot password page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ForgotPasswordButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (Window.GetWindow(this) is not LoginWindow loginWindow) return;
            loginWindow.Frame.Content = new ForgotPasswordPage();
        }
    }
}
