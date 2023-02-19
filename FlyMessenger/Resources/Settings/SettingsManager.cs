using System;
using System.Windows.Media.Animation;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FlyMessenger.Resources.Languages;
using FlyMessenger.Resources.Settings.Pages;
using Application = System.Windows.Application;

namespace FlyMessenger.Resources.Settings
{
    public class SettingsPage
    {
        public int? Id { get; }
        public string? Title { get; }
        public Page? Page { get; }
        public int? PreviousPageId { get; }

        public SettingsPage(int? id, string? title, Page? page, int? previousPageId = null)
        {
            Id = id;
            Title = title;
            Page = page;

            if (previousPageId != null) PreviousPageId = previousPageId;
        }
    }

    public abstract class SettingsManager
    {
        // Array of settings pages
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
            if (CurrentPage?.Page == null) return;

            if (Application.Current.MainWindow is not MainWindow window) return;
            var slideAnimation = new ThicknessAnimation(
                new Thickness(0),
                new Thickness(0, 0, -150, 0),
                TimeSpan.FromSeconds(0.1)
            );
            var opacityAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.1));

            slideAnimation.Completed += (s, _) => TogglePage(CurrentPage.Page);

            window.Page.BeginAnimation(FrameworkElement.MarginProperty, slideAnimation);
            window.Page.BeginAnimation(UIElement.OpacityProperty, opacityAnimation);
        }

        public static void GoBack()
        {
            var slideAnimation = new ThicknessAnimation(
                new Thickness(0),
                new Thickness(-150, 0, 0, 0),
                TimeSpan.FromSeconds(0.1)
            );
            var opacityAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.1));

            if (Application.Current.MainWindow is not MainWindow window) return;

            if (CurrentPage.PreviousPageId != null)
            {
                CurrentPage = Pages.FirstOrDefault(x => x.Id == CurrentPage.PreviousPageId);
                if (CurrentPage?.Page == null) return;

                slideAnimation.Completed += (s, _) => TogglePage(CurrentPage.Page, true);
                window.Page.BeginAnimation(FrameworkElement.MarginProperty, slideAnimation);
                window.Page.BeginAnimation(UIElement.OpacityProperty, opacityAnimation);
                return;
            }

            CurrentPage = Pages[0];
            if (CurrentPage.Page == null) return;

            slideAnimation.Completed += (s, _) => TogglePage(CurrentPage.Page, true);
            window.Page.BeginAnimation(FrameworkElement.MarginProperty, slideAnimation);
            window.Page.BeginAnimation(UIElement.OpacityProperty, opacityAnimation);
        }

        private static void TogglePage(Page page, bool isBack = false)
        {
            if (Application.Current.MainWindow is not MainWindow window) return;

            var targetThickness = isBack
                ? new Thickness(0, 0, -150, 0)
                : new Thickness(-150, 0, 0, 0);
            var slideAnimation = new ThicknessAnimation(
                targetThickness,
                new Thickness(0),
                TimeSpan.FromSeconds(0.1)
            );
            var opacityAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.1));

            window.Page.BeginAnimation(FrameworkElement.MarginProperty, slideAnimation);
            window.Page.BeginAnimation(UIElement.OpacityProperty, opacityAnimation);

            window.ModalFrame.Content = page;
            window.SettingsWindowTitle.Text = CurrentPage.Title;
            window.GoBackButton.Visibility = CurrentPage.Id != DefaultPage.Id ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
