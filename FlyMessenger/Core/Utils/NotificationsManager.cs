using System.IO;
using System.Windows.Forms;
using FlyMessenger.MVVM.Model;
using FlyMessenger.Resources.Windows;
using Newtonsoft.Json;

namespace FlyMessenger.Core.Utils
{
    public class NotificationsManager
    {
        public static void SendNotification(dynamic json)
        {
            var sender = json.message.sender;
            var message = json.message;

            var userFirstAndLastName = (string)sender.firstName + " " + (string)sender.LastName;
            var photoUrl = (string)sender.photoURL;
            if (message.file != null)
            {
                var messageTitle = (string)message.file;
                ShowNotification(userFirstAndLastName, messageTitle, photoUrl);
            }
            else
            {
                var messageTitle = (string)message.text;
                ShowNotification(userFirstAndLastName, messageTitle, photoUrl);
            }
        }

        private static void ShowNotification(string userFirstAndLastName, string messageTitle, string photoUrl)
        {
            var notification = new NotificationWindow(userFirstAndLastName, messageTitle, photoUrl);
            notification.Show();
        }
    }
}
