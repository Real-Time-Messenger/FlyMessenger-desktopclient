using FlyMessenger.Resources.Languages;

namespace FlyMessenger.Resources.Settings.Pages
{
    /// <summary>
    /// Interaction logic for SessionManagement.xaml
    /// </summary>
    public partial class SessionManagement
    {
        public SessionManagement()
        {
            InitializeComponent();
            
            // Get sessions from MyProfile and set count of sessions
            SessionsList.Loaded += (_, _) =>
            {
                SessionsList.ItemsSource = MainWindow.MainViewModel.MyProfile.Sessions;
                SessionsCountTextBlock.Text =
                    MainWindow.MainViewModel.MyProfile.Sessions?.Count + " " + lang.sessions;
            };
        }
    }
}

