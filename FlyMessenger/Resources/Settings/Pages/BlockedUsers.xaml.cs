using FlyMessenger.Resources.Languages;

namespace FlyMessenger.Resources.Settings.Pages
{
    /// <summary>
    /// Interaction logic for BlockedUsersPage.xaml
    /// </summary>
    public partial class BlockedUsersPage
    {
        public BlockedUsersPage()
        {
            InitializeComponent();
            
            // Get blocked users from MyProfile and set count of blocked users.
            BlackListView.Loaded += (_, _) =>
            {
                BlackListView.ItemsSource = MainWindow.MainViewModel.MyProfile.BlackList;
                BlockedUsersCountTextBlock.Text =
                    MainWindow.MainViewModel.MyProfile.BlackList.Count + " " + lang.blocked_users_second;
            };
        }
    }
}
