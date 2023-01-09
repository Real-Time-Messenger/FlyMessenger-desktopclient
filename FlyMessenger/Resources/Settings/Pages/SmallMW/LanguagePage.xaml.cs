using System.Windows;
using System.Windows.Controls;

namespace FlyMessenger.Resources.Settings.Pages.SmallMW
{
    public partial class LanguagePage : Page
    {
        public LanguagePage()
        {
            InitializeComponent();
        }
        
        private void OnEstonianRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.LangSwitch = "et-EE";
            mainWindow.Close();
        }
        
        private void OnRussianRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.LangSwitch = "ru-RU";
            mainWindow.Close();
        }
        
        private void OnEnglishRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.LangSwitch = "en-US";
            mainWindow.Close();
        }
    }
}

