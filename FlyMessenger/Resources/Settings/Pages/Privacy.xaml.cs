using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace FlyMessenger.Resources.Settings.Pages
{
    public partial class PrivacyPage : Page
    {
        public PrivacyPage()
        {
            InitializeComponent();

            App.LastActivityTextData = LastActivityTextData;
        }

        private void OpenLastActivityModalWindow(object sender, MouseButtonEventArgs e)
        {
            var window = (MainWindow?)Application.Current.MainWindow;
            window!.LastActivityModalWindow.IsOpen = true;
            
            var openAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.1));
            window.LastActivityModalWindow.BeginAnimation(OpacityProperty, openAnimation);
        }
    }
}

