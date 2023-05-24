using System.Windows.Controls;
using System.Windows.Input;
using FlyMessenger.Controllers;

namespace FlyMessenger.Resources.Settings.Pages
{
    public partial class NotificationsPage : Page
    {
        public NotificationsPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handler for notification state button click.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private async void NotificationStateChanged(object sender, MouseButtonEventArgs e)
        {
            await ControllerBase.UserController.EditMyChatsNotifications(!NotificationStateButton.CheckState);
            MainWindow.MainViewModel.MyProfile.Settings.ChatsNotificationsEnabled = !NotificationStateButton.CheckState;
            SoundStateButton.CheckState = !NotificationStateButton.CheckState;
            await ControllerBase.UserController.EditMyChatsSound(!NotificationStateButton.CheckState);
        }

        /// <summary>
        /// Handler for sound state button click.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private async void SoundStateChanged(object sender, MouseButtonEventArgs e)
        {
            await ControllerBase.UserController.EditMyChatsSound(!SoundStateButton.CheckState);
            MainWindow.MainViewModel.MyProfile.Settings.ChatsSoundEnabled = !SoundStateButton.CheckState;
            if (NotificationStateButton.CheckState) return;
            NotificationStateButton.CheckState = true;
            await ControllerBase.UserController.EditMyChatsNotifications(true);
        }
    }
}
