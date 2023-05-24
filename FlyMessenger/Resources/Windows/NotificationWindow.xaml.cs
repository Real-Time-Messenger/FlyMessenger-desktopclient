using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Application = System.Windows.Application;

namespace FlyMessenger.Resources.Windows
{
    /// <summary>
    /// Interaction logic for NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow
    {
        public string userFirstAndLastName { get; set; }
        public string messageTitle { get; set; }
        public string photoUrl { get; set; }
        public string dialogId { get; set; }
        
        /// <summary>
        /// Constructor for NotificationWindow
        /// </summary>
        /// <param name="userFirstAndLastName">User first and last name</param>
        /// <param name="messageTitle">Message title</param>
        /// <param name="photoUrl">Photo url</param>
        /// <param name="dialogId">Dialog id</param>
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
            Loaded += WindowLoaded;
            Closed += WindowClosed;
        }

        /// <summary>
        /// Window loaded event
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = SystemParameters.WorkArea;
            Left = desktopWorkingArea.Right - Width - 6;
            Top = desktopWorkingArea.Bottom - Height - 7;

            // Set 3 notification windows limit. New notification window will be created on the down of the last one.
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
            
            // Check if dialog is not muted
            var dialog = MainWindow.MainViewModel.Dialogs.FirstOrDefault(d => d.Id == dialogId);
            if (dialog is { IsSoundEnabled: false }) return;
            var soundPlayer = new System.Media.SoundPlayer(Properties.Resources.PopDing);
            soundPlayer.Load();
            soundPlayer.Play();
        }

        /// <summary>
        /// Window closed event
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void WindowClosed(object sender, EventArgs e)
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

        /// <summary>
        /// Close button click event
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Notification window click event
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void NotificationWindowClick(object sender, MouseButtonEventArgs e)
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
            window.CheckMessageOnScreen();
            
            var message = dialog?.Messages.FirstOrDefault(m => m.IsRead == false);
            window.MessagesListView.ScrollIntoView(message!);
            
            Close();

            var activeChatVisibilityNone = MainWindow.MainViewModel.ActiveChatVisHidden;
            if (!activeChatVisibilityNone) return;
            MainWindow.MainViewModel.ActiveChatVisHidden = false;
            MainWindow.MainViewModel.ActiveChatVisibility = true;
        }
    }
}
