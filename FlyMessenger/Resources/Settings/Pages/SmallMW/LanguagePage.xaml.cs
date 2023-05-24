using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace FlyMessenger.Resources.Settings.Pages.SmallMW
{
    /// <summary>
    /// Interaction logic for LanguagePage.xaml
    /// </summary>
    public partial class LanguagePage : Page
    {
        public LanguagePage()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// Handler for estonian radio button click.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void OnEstonianRadioButtonClick(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is not MainWindow window) return;
            var animation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.2));
            window.RestartNotifyModalWindow.IsOpen = true;
            
            animation.Completed += (o, args) =>
            {
                window.LangSwitch = "et-EE";
            };
            
            window.RestartNotifyModalWindow.BeginAnimation(OpacityProperty, animation);
        }
        
        /// <summary>
        /// Handler for russian radio button click.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void OnRussianRadioButtonClick(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is not MainWindow window) return;
            var animation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.2));
            window.RestartNotifyModalWindow.IsOpen = true;
            
            animation.Completed += (o, args) =>
            {
                window.LangSwitch = "ru-RU";
            };
            
            window.RestartNotifyModalWindow.BeginAnimation(OpacityProperty, animation);
        }
        
        /// <summary>
        /// Handler for english radio button click.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void OnEnglishRadioButtonClick(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is not MainWindow window) return;
            var animation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.2));
            window.RestartNotifyModalWindow.IsOpen = true;
            
            animation.Completed += (o, args) =>
            {
                window.LangSwitch = "en-US";
            };
            
            window.RestartNotifyModalWindow.BeginAnimation(OpacityProperty, animation);
        }
    }
}

