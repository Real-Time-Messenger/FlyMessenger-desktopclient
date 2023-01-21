using System;
using System.Windows.Media.Animation;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;
using FlyMessenger.MVVM.ViewModels;
using FlyMessenger.Resources.Languages;
using FlyMessenger.Resources.Settings.Pages;
using MessageBox = System.Windows.Forms.MessageBox;
using Application = System.Windows.Application;

namespace FlyMessenger.Resources.Settings
{
    public class SettingsPage
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
        public Page? Page { get; set; }
        public int? PreviousPageId { get; } = null;

        public SettingsPage(int? id, string? title, Page? page, int? previousPageId = null)
        {
            Id = id;
            Title = title;
            Page = page;
            
            if (previousPageId != null) PreviousPageId = previousPageId;
        }
    }

    public class SettingsManager
    {
        // Set array of settings pages
        private static readonly SettingsPage[] Pages =
        {
            new SettingsPage(
                0,
                lang.settings,
                new DefaultPage()
            ),
            new SettingsPage(
                1,
                lang.profile,
                new ProfilePage()
            ),
            new SettingsPage(
                2,
                lang.notifications,
                new NotificationsPage()
            ),
            new SettingsPage(
                3,
                lang.privacy,
                new PrivacyPage()
            ),
            new SettingsPage(
                4,
                lang.blacklist,
                new BlockedUsersPage()
            ),
            new SettingsPage(
                7,
                lang.sessions_page,
                new SessionManagement(),
                3
            )
        };

        public static readonly SettingsPage DefaultPage = Pages[0];

        public static SettingsPage CurrentPage { get; set; } = DefaultPage;

        public static void OpenPage(int id)
        {
            CurrentPage = Pages.FirstOrDefault(x => x.Id == id);
            if (CurrentPage == null || CurrentPage.Page == null) return;
            
            TogglePage(CurrentPage.Page);
        }

        public static void GoBack()
        {
            if (CurrentPage.PreviousPageId != null)
            {
                CurrentPage = Pages.FirstOrDefault(x => x.Id == CurrentPage.PreviousPageId);
                if (CurrentPage == null || CurrentPage.Page == null) return;
                
                TogglePage(CurrentPage.Page);
                return;
            }
            
            CurrentPage = Pages[0];
            if (CurrentPage.Page == null) return;
            
            TogglePage(CurrentPage.Page);
        }

        private static void TogglePage(Page page)
        {
            var window = Application.Current.MainWindow as MainWindow;
            if (window == null) return;

            window.ModalFrame.Content = page;
            window.SettingsWindowTitle.Text = CurrentPage.Title;
            window.GoBackButton.Visibility = CurrentPage.Id != DefaultPage.Id ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}