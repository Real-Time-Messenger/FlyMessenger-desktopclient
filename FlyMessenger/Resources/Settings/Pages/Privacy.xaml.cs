using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using FlyMessenger.Controllers;

namespace FlyMessenger.Resources.Settings.Pages
{
    public partial class PrivacyPage
    {
        public PrivacyPage()
        {
            InitializeComponent();

            App.LastActivityTextData = LastActivityTextData;
        }

        private void OpenLastActivityModalWindow(object sender, MouseButtonEventArgs e)
        {
            if (Application.Current.MainWindow is not MainWindow window) return;
            window.LastActivityModalWindow.IsOpen = true;

            var openAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.1));
            window.LastActivityModalWindow.BeginAnimation(OpacityProperty, openAnimation);
        }

        private void TwoFactorStateChanged(object sender, MouseButtonEventArgs e)
        {
            ControllerBase.UserController.EditMyTwoFactorAuth(!TwoFactorButton.CheckState);
            MainWindow.MainViewModel.MyProfile.Settings.TwoFactorEnabled = !TwoFactorButton.CheckState;
        }

        private void AutoStartStateChanged(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                    "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run",
                    true
                );
                var curAssembly = Assembly.GetExecutingAssembly();
                var curAssemblyName = curAssembly.GetName().Name;
                var curAssemblyPath = curAssembly.Location;
                if (AutoStartButton.CheckState)
                {
                    if (curAssemblyName != null) key?.DeleteValue(curAssemblyName);
                }
                else
                {
                    key?.SetValue(curAssemblyName, curAssemblyPath);
                }
                ControllerBase.UserController.EditMyAutoStart(!AutoStartButton.CheckState);
                Properties.Settings.Default.RunOnStartupAllowed = !AutoStartButton.CheckState;
                Properties.Settings.Default.Save();

                MainWindow.MainViewModel.AutoStartupEnabled = !AutoStartButton.CheckState;
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "Error while changing auto start state",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }
}
