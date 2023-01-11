using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace FlyMessenger.Resources.Settings.Pages.SmallMW
{
    public partial class LanguagePage : Page
    {
        public LanguagePage()
        {
            InitializeComponent();
        }
        
        private void OnEstonianRadioButtonClick(object sender, RoutedEventArgs e)
        {
            var window = (MainWindow)Application.Current.MainWindow!;
            var animation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.2));
            window.RestartNotifyModalWindow.IsOpen = true;
            
            animation.Completed += (o, args) =>
            {
                window.LangSwitch = "et-EE";
            };
            
            window.RestartNotifyModalWindow.BeginAnimation(OpacityProperty, animation);
        }
        
        private void OnRussianRadioButtonClick(object sender, RoutedEventArgs e)
        {
            var window = (MainWindow)Application.Current.MainWindow!;
            var animation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.2));
            window.RestartNotifyModalWindow.IsOpen = true;
            
            animation.Completed += (o, args) =>
            {
                window.LangSwitch = "ru-RU";
            };
            
            window.RestartNotifyModalWindow.BeginAnimation(OpacityProperty, animation);
        }
        
        private void OnEnglishRadioButtonClick(object sender, RoutedEventArgs e)
        {
            var window = (MainWindow)Application.Current.MainWindow!;
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

