using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using FlyMessenger.Controllers;
using FlyMessenger.Core.Utils;
using FlyMessenger.HTTP;
using FlyMessenger.Properties;
using FlyMessenger.Resources.Authorization;

namespace FlyMessenger
{
    public partial class LoginWindow
    {
        public string? CurLanguage { get; set; }
        public Page CurPage { get; set; } = new LoginPage();
        private bool _isOpened;

        public LoginWindow()
        {
            CurLanguage = Settings.Default.LanguageCode switch
            {
                "en-US" => "English",
                "" => "Eesti",
                "et-EE" => "Eesti",
                "ru-RU" => "Русский",
                _ => CurLanguage
            };

            InitializeComponent();

            Loaded += LoginWindow_Loaded;
            
            var httpToken = HttpClientBase.GetToken();
            var token = new TokenSettings().Load();
            if (token == string.Empty || httpToken != "Bearer " + token) return;
            var myProfile = ControllerBase.UserController.GetMyProfile();
            if (myProfile == null) return;
            Application.Current.MainWindow = new MainWindow();
            Application.Current.MainWindow.Show();
            Application.Current.MainWindow.Activate();
            Close();
        }

        private void LoginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var curTheme = Settings.Default.CurrentTheme;

            ThemeImage.SetResourceReference(
                Image.SourceProperty,
                curTheme == "Light" ? "ThemeIcon-Moon" : "ThemeIcon-Sun"
            );
        }

        private void ThemeButton_Click(object sender, RoutedEventArgs e)
        {
            var resources = Application.Current.Resources.MergedDictionaries;

            // Clear all resources
            resources.Clear();

            // Add new resources
            if (Settings.Default.CurrentTheme == "Light")
            {
                resources.Add(
                    new ResourceDictionary
                    {
                        Source = new Uri("pack://application:,,,/Resources/Colors/Dark.xaml")
                    }
                );

                Settings.Default.CurrentTheme = "Dark";
                Settings.Default.Save();

                ThemeImage.SetResourceReference(Image.SourceProperty, "ThemeIcon-Sun");
            }
            else
            {
                resources.Add(
                    new ResourceDictionary
                    {
                        Source = new Uri("pack://application:,,,/Resources/Colors/Light.xaml")
                    }
                );

                Settings.Default.CurrentTheme = "Light";
                Settings.Default.Save();

                ThemeImage.SetResourceReference(Image.SourceProperty, "ThemeIcon-Moon");
            }
        }

        private void LanguageButton_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_isOpened) return;
            LanguagePanel.Visibility = Visibility.Visible;
            ToggleAnimation();
        }

        private void LanguagePanel_MouseLeave(object sender, MouseEventArgs e)
        {
            HideAnimation();
        }

        private void EnglishButton_Click(object sender, RoutedEventArgs e)
        {
            const string language = "en-US";
            Settings.Default.LanguageCode = language;
            Settings.Default.Save();
            App.RestartApp();
        }

        private void EstonianButton_Click(object sender, RoutedEventArgs e)
        {
            const string language = "et-EE";
            Settings.Default.LanguageCode = language;
            Settings.Default.Save();
            App.RestartApp();
        }

        private void RussianButton_Click(object sender, RoutedEventArgs e)
        {
            const string language = "ru-RU";
            Settings.Default.LanguageCode = language;
            Settings.Default.Save();
            App.RestartApp();
        }

        private void ToggleAnimation()
        {
            var heightAnimationIn = new DoubleAnimation(
                0,
                106,
                TimeSpan.FromMilliseconds(200)
            );

            _isOpened = true;

            LanguagePanel.BeginAnimation(HeightProperty, heightAnimationIn);
        }

        private void HideAnimation()
        {
            var heightAnimationOut = new DoubleAnimation(
                LanguagePanel.ActualHeight,
                0,
                TimeSpan.FromMilliseconds(200)
            );

            heightAnimationOut.Completed += (_, _) =>
            {
                LanguagePanel.Visibility = Visibility.Collapsed;
                _isOpened = false;
            };

            LanguagePanel.BeginAnimation(HeightProperty, heightAnimationOut);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void WindowDrag(object sender, MouseButtonEventArgs e)
        {
            // if not sender return
            if (e.Source != sender) return;
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}
