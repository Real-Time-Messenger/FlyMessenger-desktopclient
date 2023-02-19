using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using FlyMessenger.Controllers;

namespace FlyMessenger.Resources.Settings.Pages
{
    public partial class NotificationsPage : Page
    {
        public NotificationsPage()
        {
            InitializeComponent();
        }

        private void NotificationStateChanged(object sender, MouseButtonEventArgs e)
        {
            ControllerBase.UserController.EditMyChatsNotifications(!NotificationStateButton.CheckState);
            MainWindow.MainViewModel.MyProfile.Settings.ChatsNotificationsEnabled = !NotificationStateButton.CheckState;
            SoundStateButton.CheckState = !NotificationStateButton.CheckState;
            ControllerBase.UserController.EditMyChatsSound(!NotificationStateButton.CheckState);
        }

        private void SoundStateChanged(object sender, MouseButtonEventArgs e)
        {
            ControllerBase.UserController.EditMyChatsSound(!SoundStateButton.CheckState);
            MainWindow.MainViewModel.MyProfile.Settings.ChatsSoundEnabled = !SoundStateButton.CheckState;
            if (NotificationStateButton.CheckState) return;
            NotificationStateButton.CheckState = true;
            ControllerBase.UserController.EditMyChatsNotifications(true);
        }
    }
}
