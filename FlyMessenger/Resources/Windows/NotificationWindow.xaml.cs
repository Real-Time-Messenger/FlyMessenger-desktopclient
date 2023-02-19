using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Application = System.Windows.Application;

namespace FlyMessenger.Resources.Windows
{
    public partial class NotificationWindow
    {
        public NotificationWindow(string userFirstAndLastName, string messageTitle, string photoUrl, string dialogId)
        {
            this.userFirstAndLastName = userFirstAndLastName;
            this.messageTitle = messageTitle;
            this.photoUrl = photoUrl;
            this.dialogId = dialogId;

            ShowInTaskbar = false;
            Topmost = true;
            ShowActivated = false;


            InitializeComponent();
            Loaded += Window_Loaded;
            Closed += Window_Closed;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = SystemParameters.WorkArea;
            Left = desktopWorkingArea.Right - Width - 6;
            Top = desktopWorkingArea.Bottom - Height - 7;

            // if more than one notification is sent, the new one will be shown down. Maximum is 3 notifications.
            var notificationWindows = Application.Current.Windows.OfType<NotificationWindow>().ToList();
            switch (notificationWindows.Count)
            {
                case 1:
                    Top = desktopWorkingArea.Bottom - Height - 7;
                    break;
                case 2:
                    // Set first notification window to the top of the second one.
                    notificationWindows[0].Top = desktopWorkingArea.Bottom - Height - 7 - Height - 7;
                    Top = desktopWorkingArea.Bottom - Height - 7;
                    break;
                case 3:
                    // Set first notification window to the top of the second one, and second notification window to the top of the third one.
                    notificationWindows[0].Top = desktopWorkingArea.Bottom - Height - 7 - Height - 7 - Height - 7;
                    notificationWindows[1].Top = desktopWorkingArea.Bottom - Height - 7 - Height - 7;
                    Top = desktopWorkingArea.Bottom - Height - 7;
                    break;
                case 4:
                    // Delete first notification window, and set second notification window to the top of the third one,
                    // and third notification window to the top of the fourth one.
                    notificationWindows[0].Close();
                    notificationWindows[1].Top = desktopWorkingArea.Bottom - Height - 7 - Height - 7 - Height - 7;
                    notificationWindows[2].Top = desktopWorkingArea.Bottom - Height - 7 - Height - 7;
                    Top = desktopWorkingArea.Bottom - Height - 7;
                    break;
            }

            // Toggle sound effect
            if (!MainWindow.MainViewModel.MyProfile.Settings.ChatsSoundEnabled) return;
            // if isSoundEnabled is false on dialog, then don't play sound.
            var dialog = MainWindow.MainViewModel.Dialogs.FirstOrDefault(d => d.Id == dialogId);
            if (dialog is { IsSoundEnabled: false }) return;
            var soundPlayer = new System.Media.SoundPlayer(Properties.Resources.PopDing);
            soundPlayer.Load();
            soundPlayer.Play();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            var desktopWorkingArea = SystemParameters.WorkArea;
            Left = desktopWorkingArea.Right - Width - 6;
            Top = desktopWorkingArea.Bottom - Height - 7;

            var notificationWindows = Application.Current.Windows.OfType<NotificationWindow>().ToList();
            switch (notificationWindows.Count)
            {
                // If I have 3 notifications, and I want close the first one or second, then notification what upper than the closed one will be moved down.
                case 1:
                    notificationWindows[0].Top = desktopWorkingArea.Bottom - Height - 7;
                    break;
                case 2:
                    notificationWindows[^1].Top = desktopWorkingArea.Bottom - Height - 7;
                    notificationWindows[^2].Top = desktopWorkingArea.Bottom - Height - 7 - Height - 7;
                    break;
            }
        }

        public string userFirstAndLastName { get; set; }
        public string messageTitle { get; set; }
        public string photoUrl { get; set; }
        public string dialogId { get; set; }
        
        private void Button_Click_Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void NotificationWindow_Click(object sender, MouseButtonEventArgs e)
        {
            // if NotificationCloseButton then return
            if (e.Source is System.Windows.Controls.Button) return;
            
            if (Application.Current.MainWindow is not MainWindow window) return;
            
            window.Show();
            window.Activate();
            if (window.WindowState == WindowState.Minimized)
            {
                window.WindowState = WindowState.Normal;
            }
            
            var dialog = MainWindow.MainViewModel.Dialogs.FirstOrDefault(d => d.Id.Equals(dialogId));
            MainWindow.MainViewModel.SelectedDialog = dialog!;
            
            if (window.MessagesListView.Items.IsEmpty) return;
            window.MainWindow_CheckMessageOnScreen();
            
            var message = dialog?.Messages.FirstOrDefault(m => m.IsRead == false);
            window.MessagesListView.ScrollIntoView(message!);
            
            Close();

            var activeChatVisibilityNone = MainWindow.MainViewModel.ActiveChatVisibilityNone;
            if (!activeChatVisibilityNone) return;
            MainWindow.MainViewModel.ActiveChatVisibilityNone = false;
            MainWindow.MainViewModel.ActiveChatVisibility = true;
        }
    }
}
