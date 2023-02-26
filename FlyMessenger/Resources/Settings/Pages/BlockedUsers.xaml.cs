using FlyMessenger.Resources.Languages;

namespace FlyMessenger.Resources.Settings.Pages
{
    public partial class BlockedUsersPage
    {
        public BlockedUsersPage()
        {
            InitializeComponent();
            
            BlackListView.Loaded += (_, _) =>
            {
                BlackListView.ItemsSource = MainWindow.MainViewModel.MyProfile.BlackList;
                BlockedUsersCountTextBlock.Text =
                    MainWindow.MainViewModel.MyProfile.BlackList.Count + " " + lang.blocked_users_second;
            };
        }
    }
}
