using FlyMessenger.Resources.Windows;
using Application = System.Windows.Application;

namespace FlyMessenger.Core.Utils
{
    public static class NotificationsManager
    {
        public static void SendNotification(dynamic json)
        {
            Application.Current.Dispatcher.Invoke(
                () =>
                {
                    var sender = json.message.sender;
                    if ((string)sender.id == MainWindow.MainViewModel.MyProfile.Id) return;

                    var message = json.message;
                    var userFirstAndLastName = (string)sender.firstName + " " + (string)sender.LastName;
                    var photoUrl = (string)sender.photoURL;
                    var dialogId = (string)message.dialogId;

                    if (message.file != null)
                    {
                        var messageTitle = (string)message.file;
                        ShowNotification(userFirstAndLastName, messageTitle, photoUrl, dialogId);
                    }
                    else
                    {
                        var messageTitle = (string)message.text;
                        ShowNotification(userFirstAndLastName, messageTitle, photoUrl, dialogId);
                    }
                }
            );
        }

        public static void CloseAllNotifications()
        {
            foreach (var notificationWindow in Application.Current.Windows)
            {
                if (notificationWindow is not NotificationWindow window) return;
                window.Close();
            }
        }

        private static void ShowNotification(string userFirstAndLastName, string messageTitle, string photoUrl, string dialogId)
        {
            Application.Current.Dispatcher.Invoke(
                () =>
                {
                    var notificationWindow = new NotificationWindow(
                        userFirstAndLastName,
                        messageTitle,
                        photoUrl,
                        dialogId
                    );
                    notificationWindow.Show();
                }
            );
        }
    }
}
