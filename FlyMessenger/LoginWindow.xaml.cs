using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using FlyMessenger.Controllers;
using FlyMessenger.Core.Utils;
using FlyMessenger.Properties;
using FlyMessenger.Resources.Authorization;

namespace FlyMessenger
{
    /// <summary>
    /// Logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow
    {
        public string? CurLanguage { get; set; }
        public Page CurPage { get; set; } = new LoginPage();
        private bool _isOpened;

        /// <summary>
        /// Constructor for LoginWindow
        /// </summary>
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

            Loaded += LoginWindowLoaded;
            
            var token = new TokenSettings().Load();
            if (token == string.Empty) return;
            var myProfile = ControllerBase.UserController.GetMyProfile();
            if (myProfile.Id == null) return;
            Application.Current.MainWindow = new MainWindow();
            Application.Current.MainWindow.Show();
            Application.Current.MainWindow.Activate();
            Close();
        }

        /// <summary>
        /// Loaded event for LoginWindow
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void LoginWindowLoaded(object sender, RoutedEventArgs e)
        {
            var curTheme = Settings.Default.CurrentTheme;

            ThemeImage.SetResourceReference(
                Image.SourceProperty,
                curTheme == "Light" ? "ThemeIcon-Moon" : "ThemeIcon-Sun"
            );
        }

        /// <summary>
        /// Handle theme button click
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void ThemeButtonClick(object sender, RoutedEventArgs e)
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

        /// <summary>
        /// Handle language button mouse enter
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void LanguageButtonMouseEnter(object sender, MouseEventArgs e)
        {
            if (_isOpened) return;
            LanguagePanel.Visibility = Visibility.Visible;
            ToggleAnimation();
        }

        /// <summary>
        /// Handle language button mouse leave
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void LanguagePanelMouseLeave(object sender, MouseEventArgs e)
        {
            HideAnimation();
        }

        /// <summary>
        /// Handle english language button click
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void EnglishButtonClick(object sender, RoutedEventArgs e)
        {
            const string language = "en-US";
            Settings.Default.LanguageCode = language;
            Settings.Default.Save();
            App.RestartApp();
        }

        /// <summary>
        /// Handle estonian language button click
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void EstonianButtonClick(object sender, RoutedEventArgs e)
        {
            const string language = "et-EE";
            Settings.Default.LanguageCode = language;
            Settings.Default.Save();
            App.RestartApp();
        }

        /// <summary>
        /// Handle russian language button click
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void RussianButtonClick(object sender, RoutedEventArgs e)
        {
            const string language = "ru-RU";
            Settings.Default.LanguageCode = language;
            Settings.Default.Save();
            App.RestartApp();
        }

        /// <summary>
        /// Method for toggle animation
        /// </summary>
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

        /// <summary>
        /// Method for hide animation
        /// </summary>
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

        /// <summary>
        /// Handle close button click
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Method for window drag
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void WindowDrag(object sender, MouseButtonEventArgs e)
        {
            // if not sender return
            if (e.Source != sender) return;
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}
