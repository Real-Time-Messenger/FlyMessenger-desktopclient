using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace FlyMessenger.Resources.Settings.Pages
{
    public partial class DefaultPage
    {
        public DefaultPage()
        {
            InitializeComponent();
            App.ProfilePhotoDefaultPage = ProfilePhoto;
        }

        private void Username_CopyToClipboard(object sender, MouseButtonEventArgs e)
        {
            if (sender is not TextBlock textBlock) return;

            if (Application.Current.MainWindow is not MainWindow window || window.UsernameCopiedTip.IsOpen) return;

            window.UsernameCopiedTip.IsOpen = true;

            Clipboard.SetText(textBlock.Text);

            var openAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.2));
            var closeAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.2))
            {
                BeginTime = TimeSpan.FromSeconds(2)
            };

            openAnimation.Completed += (_, _) => window.UsernameCopiedTip.BeginAnimation(OpacityProperty, closeAnimation);
            closeAnimation.Completed += (_, _) => window.UsernameCopiedTip.IsOpen = false;

            window.UsernameCopiedTip.BeginAnimation(OpacityProperty, openAnimation);
        }

        private void OpenLanguage_ModalWindow(object sender, MouseButtonEventArgs e)
        {
            if (Application.Current.MainWindow is not MainWindow window) return;
            window.LanguageModalWindow.IsOpen = true;

            var openAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.1));
            window.LanguageModalWindow.BeginAnimation(OpacityProperty, openAnimation);
        }

        private void OpenDeleteAccount_ModalWindow(object sender, MouseButtonEventArgs e)
        {
            if (Application.Current.MainWindow is not MainWindow window) return;
            window.DeleteAccountModalWindow.IsOpen = true;

            var openAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.1));
            window.DeleteAccountModalWindow.BeginAnimation(OpacityProperty, openAnimation);
        }
    }
}
