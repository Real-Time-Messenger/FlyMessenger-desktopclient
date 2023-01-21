using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;

namespace FlyMessenger.Resources.Windows
{
    public partial class NotificationWindow : Window
    {
        public NotificationWindow(string userFirstAndLastName, string messageTitle, string photoUrl)
        {
            this.userFirstAndLastName = userFirstAndLastName;
            this.messageTitle = messageTitle;
            this.photoUrl = photoUrl;
            
            InitializeComponent();
            Loaded += Window_Loaded;
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = SystemParameters.WorkArea;
            Left = desktopWorkingArea.Right - Width - 6;
            Top = desktopWorkingArea.Bottom - Height - 7;
        }
        
        public string userFirstAndLastName { get; set; }
        public string messageTitle { get; set; }
        public string photoUrl { get; set; }
        
        private void Button_Click_Close(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

