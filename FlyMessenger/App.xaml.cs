using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using FlyMessenger.Controllers;
using FlyMessenger.MVVM.Model;
using FlyMessenger.Properties;
using FlyMessenger.Resources.Languages;
using FlyMessenger.Resources.Settings;
using FlyMessenger.Resources.Settings.Pages;
using FlyMessenger.Resources.Windows;
using FlyMessenger.UserControls;
using Newtonsoft.Json;
using MessageBox = System.Windows.MessageBox;

namespace FlyMessenger
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public static ImageBrush ProfilePhotoDefaultPage { get; set; }
        public static ImageBrush ProfilePhotoMainWindow { get; set; }
        public static ProfileButtons LastActivityTextData { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var processName = Process.GetCurrentProcess().ProcessName;
            if (Process.GetProcessesByName(processName).Length > 1)
            {
                Environment.Exit(0);
            }
            else
            {
                var langCode = Settings.Default.LanguageCode;
                Thread.CurrentThread.CurrentUICulture = langCode != "" ? new CultureInfo(langCode) : new CultureInfo("");

                var curTheme = Settings.Default.CurrentTheme;
                var dict = new ResourceDictionary
                {
                    Source = new Uri("Resources/Colors/" + curTheme + ".xaml", UriKind.Relative)
                };
                Resources.MergedDictionaries.Add(dict);
                ShutdownMode = ShutdownMode.OnExplicitShutdown;
            }

            base.OnStartup(e);
        }
        
        internal abstract class LanguageUtils
        {
            public static string? GetTranslation(string key)
            {
                var resourceManager = new ResourceManager(typeof(lang));
                return resourceManager.GetString(key);
            }
        }

        public static void ToggleLanguage(object? sender, EventArgs e)
        {
            if (sender is not MainWindow window || string.IsNullOrEmpty(window.LangSwitch)) return;

            var language = window.LangSwitch;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
            Thread.CurrentThread.CurrentCulture = new CultureInfo(language);
            Settings.Default.LanguageCode = language;
            Settings.Default.Save();
        }

        public static void RestartApp()
        {
            var processName = Process.GetCurrentProcess().ProcessName;
            Process.Start(processName + ".exe");
            Current.Shutdown();
        }

        public App()
        {
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        private static void OnDispatcherUnhandledException(object sender,
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is not null)
            {
                MessageBox.Show(
                    "Unhandled exception occurred: \n" + e.Exception,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void OnCloseSessionClick(object sender, RoutedEventArgs e)
        {
            var session = ((SessionsModel)((Button)sender).DataContext).Id;
            var message = new DestroySessionModel
            {
                type = "DESTROY_SESSION",
                sessionId = session
            };

            if (Current.MainWindow is not MainWindow window) return;
            window.WebSocketClient.Send(JsonConvert.SerializeObject(message));

            var mainViewModel = FlyMessenger.MainWindow.MainViewModel;
            mainViewModel.SessionsCount--;

            if (SettingsManager.CurrentPage.Page is not SessionManagement sessionManagement) return;
            mainViewModel.MyProfile.Sessions?.Remove(
                mainViewModel.MyProfile.Sessions.FirstOrDefault(x => x?.Id == session)
            );
            sessionManagement.SessionsList.ItemsSource = mainViewModel.MyProfile.Sessions;
            sessionManagement.SessionsCountTextBlock.Text =
                mainViewModel.MyProfile.Sessions?.Count + " " + lang.sessions;
        }

        private void OnUnblockUserClick(object sender, RoutedEventArgs e)
        {
            var userId = ((BlackListModel)((Button)sender).DataContext).Id;
            var result = ControllerBase.UserController.BlockOrUnblockUser(userId);
            FlyMessenger.MainWindow.MainViewModel.MyProfile.BlackList = result.BlackList;
            FlyMessenger.MainWindow.MainViewModel.BlockedUsersCount = result.BlackList.Count;
            var dialog = FlyMessenger.MainWindow.MainViewModel.Dialogs.FirstOrDefault(d => d.User.Id == userId);
            if (dialog != null)
            {
                dialog.User.IsBlocked = result.IsBlocked;
                FlyMessenger.MainWindow.MainViewModel.SelectedDialogTextBoxVisibility = !result.IsBlocked;
            }

            if (Current.MainWindow is not MainWindow window) return;
            window.ChatBoxListView.Items.Refresh();

            if (SettingsManager.CurrentPage.Page is not BlockedUsersPage blockedUsersPage) return;
            blockedUsersPage.BlackListView.ItemsSource = result.BlackList;
            blockedUsersPage.BlockedUsersCountTextBlock.Text = result.BlackList.Count + " " + lang.blocked_users_second;
        }

        private void DialogPinState(object sender, MouseButtonEventArgs e)
        {
            var dialog = (DialogModel)((MenuItem)sender).DataContext;
            ControllerBase.DialogController.EditDialog(
                dialog.Id,
                new DialogInEdit { IsPinned = !dialog.IsPinned }
            );
            dialog.IsPinned = !dialog.IsPinned;

            if (Current.MainWindow is not MainWindow window) return;
            window.ChatBoxListView.Items.Refresh();
        }

        private void DialogNotificationsState(object sender, MouseButtonEventArgs e)
        {
            var dialog = (DialogModel)((MenuItem)sender).DataContext;
            ControllerBase.DialogController.EditDialog(
                dialog.Id,
                new DialogInEdit { IsNotificationsEnabled = !dialog.IsNotificationsEnabled }
            );
            dialog.IsNotificationsEnabled = !dialog.IsNotificationsEnabled;
            if (Current.MainWindow is not MainWindow window) return;
            window.ChatBoxListView.Items.Refresh();
        }

        private void DialogSoundState(object sender, MouseButtonEventArgs e)
        {
            var dialog = (DialogModel)((MenuItem)sender).DataContext;
            ControllerBase.DialogController.EditDialog(
                dialog.Id,
                new DialogInEdit { IsSoundEnabled = !dialog.IsSoundEnabled }
            );
            dialog.IsSoundEnabled = !dialog.IsSoundEnabled;
            if (Current.MainWindow is not MainWindow window) return;
            window.ChatBoxListView.Items.Refresh();
        }

        private void DialogBlockState(object sender, MouseButtonEventArgs e)
        {
            var userId = ((DialogModel)((MenuItem)sender).DataContext).User.Id;
            var result = ControllerBase.UserController.BlockOrUnblockUser(userId);
            FlyMessenger.MainWindow.MainViewModel.MyProfile.BlackList = result.BlackList;
            FlyMessenger.MainWindow.MainViewModel.BlockedUsersCount = result.BlackList.Count;
            var dialog = FlyMessenger.MainWindow.MainViewModel.Dialogs.FirstOrDefault(d => d.User.Id == userId);
            if (dialog != null)
            {
                dialog.User.IsBlocked = result.IsBlocked;
                FlyMessenger.MainWindow.MainViewModel.SelectedDialogTextBoxVisibility = !result.IsBlocked;
            }
            if (Current.MainWindow is not MainWindow window) return;
            window.ChatBoxListView.Items.Refresh();
        }

        private void DialogDelete(object sender, MouseButtonEventArgs e)
        {
            if (Current.MainWindow is not MainWindow window) return;
            var listView = window.ChatBoxListView;
            ControllerBase.DialogController.DeleteDialog(((DialogModel)((MenuItem)sender).DataContext).Id);
            FlyMessenger.MainWindow.MainViewModel.Dialogs.Remove((DialogModel)((MenuItem)sender).DataContext);
            listView.Items.Refresh();
        }

        private void OpenDialogGallery(object sender, MouseButtonEventArgs e)
        {
            var dialog = FlyMessenger.MainWindow.MainViewModel.SelectedDialog;
            var currentPhoto = ((MessageModel)((Image)sender).DataContext).File;
            if (currentPhoto == null) return;
            var galleryWindow = new GalleryWindow(dialog, currentPhoto);
            galleryWindow.Show();
        }
    }
}
