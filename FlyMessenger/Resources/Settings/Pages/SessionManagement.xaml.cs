using System.Collections.ObjectModel;
using FlyMessenger.MVVM.Model;
using FlyMessenger.Resources.Languages;

namespace FlyMessenger.Resources.Settings.Pages
{
    public partial class SessionManagement
    {
        public SessionManagement()
        {
            InitializeComponent();
            
            SessionsList.Loaded += (_, _) =>
            {
                SessionsList.ItemsSource = MainWindow.MainViewModel.MyProfile.Sessions;
                SessionsCountTextBlock.Text =
                    MainWindow.MainViewModel.MyProfile.Sessions?.Count + " " + lang.sessions;
            };
        }
    }
}

