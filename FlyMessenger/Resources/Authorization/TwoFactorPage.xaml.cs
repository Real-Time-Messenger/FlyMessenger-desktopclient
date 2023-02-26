using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using FlyMessenger.Controllers;
using FlyMessenger.HTTP;

namespace FlyMessenger.Resources.Authorization
{
    public partial class TwoFactorPage
    {
        public TwoFactorPage()
        {
            InitializeComponent();
        }

        private void CodeSetActive(object sender, MouseButtonEventArgs e)
        {
            var animation = new DoubleAnimation(CodeLabel.Opacity, 1, TimeSpan.FromSeconds(0.1));
            var animationBorder = new ThicknessAnimation(
                new Thickness(0, 0, 0, CodeTextBoxBorder.BorderThickness.Bottom),
                new Thickness(0, 0, 0, 2),
                TimeSpan.FromSeconds(0.1)
            );

            CodeLabel.BeginAnimation(OpacityProperty, animation);
            CodeTextBoxBorder.BeginAnimation(Control.BorderThicknessProperty, animationBorder);
        }

        private async void LoginButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(CodeTextBox.Text))
            {
                CodeErrorLabel.Visibility = Visibility.Visible;
                CodeErrorLabel.Text = App.LanguageUtils.GetTranslation("twoFactorCodeIsRequired");
                return;
            }
            CodeErrorLabel.Visibility = Visibility.Collapsed;
            ShowLoginButton(false);
            var result = await ControllerBase.UserController.TwoFactorAuthenticate(CodeTextBox.Text);

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
                    break;
                default:
                    ShowLoginButton(true);
                    
                    if (result.Data?.Details != null)
                        foreach (var detail in result.Data.Details)
                        {
                            if (detail.Field != "code") continue;
                            CodeErrorLabel.Visibility = Visibility.Visible;
                            CodeErrorLabel.Text = App.LanguageUtils.GetTranslation(detail.Translation!);
                        }
                    
                    LoginErrorTextBlock.Visibility = Visibility.Visible;
                    if (result.Data?.Translation != null)
                    {
                        LoginErrorTextBlock.Text = App.LanguageUtils.GetTranslation(result.Data.Translation);
                    }
                    break;
            }
        }

        private void ShowLoginButton(bool show)
        {
            LoginButton.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            LoginButtonLoading.Visibility = show ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
