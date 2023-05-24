using System.Linq;
using FlyMessenger.Resources.Languages;
using FlyMessenger.Resources.Windows;
using Application = System.Windows.Application;

namespace FlyMessenger.Core.Utils
{
    /// <summary>
    /// Notifications manager.
    /// </summary>
    public static class NotificationsManager
    {
        // Send notification.
        public static void SendNotification(dynamic json)
        {
            // Check if notification is enabled.
            Application.Current.Dispatcher.Invoke(
                () =>
                {
                    // Get sender.
                    var sender = json.message.sender;
                    if ((string)sender.id == MainWindow.MainViewModel.MyProfile.Id) return;

                    // Get message.
                    var message = json.message;
                    var userFirstAndLastName = (string)sender.firstName + " " + (string)sender.LastName;
                    var photoUrl = (string)sender.photoURL;
                    var dialogId = (string)message.dialogId;

                    // Check if message is a file.
                    if (message.file != null)
                    {
                        ShowNotification(userFirstAndLastName, lang.photo, photoUrl, dialogId);
                    }
                    else
                    {
                        var messageTitle = (string)message.text;
                        ShowNotification(userFirstAndLastName, messageTitle, photoUrl, dialogId);
                    }
                }
            );
        }

        // Close all notifications.
        public static void CloseAllNotifications()
        {
            foreach (var notificationWindow in Application.Current.Windows)
            {
                // Check if window is notification window.
                if (notificationWindow is not NotificationWindow window) return;
                window.Close();
            }
        }
        
        // Close dialog notifications.
        public static void CloseDialogNotifications(string dialogId)
        {
            // Close all notifications with same dialogId.
            foreach (var notificationWindow in Application.Current.Windows.OfType<NotificationWindow>())
            {
                if (notificationWindow.dialogId == dialogId)
                    notificationWindow.Close();
            }
        }

        // Show notification.
        private static void ShowNotification(string userFirstAndLastName, string messageTitle, string photoUrl, string dialogId)
        {
            // Invoke notification window.
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
