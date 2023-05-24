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
        public static ImageBrush SettingsProfilePhoto { get; set; }
        public static ImageBrush NavBarProfilePhoto { get; set; }
        public static ProfileButtons LastActivityTextData { get; set; }

        /// <summary>
        /// On startup event
        /// </summary>
        /// <param name="e">Startup event arguments</param>
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

        /// <summary>
        /// Language utils
        /// </summary>
        internal abstract class LanguageUtils
        {
            /// <summary>
            /// Get translation by key
            /// </summary>
            /// <param name="key">Translation key</param>
            /// <returns>Translation</returns>
            public static string? GetTranslation(string key)
            {
                var resourceManager = new ResourceManager(typeof(lang));
                return resourceManager.GetString(key);
            }
        }

        /// <summary>
        /// Toggle language
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        public static void ToggleLanguage(object? sender, EventArgs e)
        {
            if (sender is not MainWindow window || string.IsNullOrEmpty(window.LangSwitch)) return;

            var language = window.LangSwitch;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
            Thread.CurrentThread.CurrentCulture = new CultureInfo(language);
            Settings.Default.LanguageCode = language;
            Settings.Default.Save();
        }

        /// <summary>
        /// Restart app
        /// </summary>
        public static void RestartApp()
        {
            var processName = Process.GetCurrentProcess().ProcessName;
            Process.Start(processName + ".exe");
            Current.Shutdown();
        }

        /// <summary>
        /// Exception handler
        /// </summary>
        public App()
        {
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        // Debug
        /*private static void OnDispatcherUnhandledException(object sender,
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
        }*/

        // Release
        private static void OnDispatcherUnhandledException(object sender,
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is null) return;
            if (MessageBox.Show(
                    "Something went wrong. Please, restart the app.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                ) == MessageBoxResult.OK)
            {
                RestartApp();
            }
        }

        /// <summary>
        /// Handle session close button click
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void CloseSessionClick(object sender, RoutedEventArgs e)
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

        /// <summary>
        /// Handle unblock user button click
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private async void UnblockUserClick(object sender, RoutedEventArgs e)
        {
            var userId = ((BlackListModel)((Button)sender).DataContext).Id;
            var result = await ControllerBase.UserController.BlockOrUnblockUser(userId);
            FlyMessenger.MainWindow.MainViewModel.MyProfile.BlackList = result.BlackList;
            FlyMessenger.MainWindow.MainViewModel.BlockedUsersCount = result.BlackList.Count;
            var dialog = FlyMessenger.MainWindow.MainViewModel.Dialogs.FirstOrDefault(d => d.User.Id == userId);
            if (dialog != null)
            {
                dialog.User.IsBlocked = result.IsBlocked;
                FlyMessenger.MainWindow.MainViewModel.DialogInputVisibility = !result.IsBlocked;
            }

            if (Current.MainWindow is not MainWindow window) return;
            window.ChatBoxListView.Items.Refresh();

            if (SettingsManager.CurrentPage.Page is not BlockedUsersPage blockedUsersPage) return;
            blockedUsersPage.BlackListView.ItemsSource = result.BlackList;
            blockedUsersPage.BlockedUsersCountTextBlock.Text = result.BlackList.Count + " " + lang.blocked_users_second;
        }

        /// <summary>
        /// Handle dialog pin state change
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private async void DialogPinState(object sender, MouseButtonEventArgs e)
        {
            var dialog = (DialogModel)((MenuItem)sender).DataContext;
            await ControllerBase.DialogController.EditDialog(
                dialog.Id,
                new DialogInEdit { IsPinned = !dialog.IsPinned }
            );
            dialog.IsPinned = !dialog.IsPinned;

            if (Current.MainWindow is not MainWindow window) return;
            window.ChatBoxListView.Items.Refresh();
        }

        /// <summary>
        /// Handle dialog notifications state change
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private async void DialogNotificationsState(object sender, MouseButtonEventArgs e)
        {
            var dialog = (DialogModel)((MenuItem)sender).DataContext;
            await ControllerBase.DialogController.EditDialog(
                dialog.Id,
                new DialogInEdit { IsNotificationsEnabled = !dialog.IsNotificationsEnabled }
            );
            dialog.IsNotificationsEnabled = !dialog.IsNotificationsEnabled;
            if (Current.MainWindow is not MainWindow window) return;
            window.ChatBoxListView.Items.Refresh();
        }

        /// <summary>
        /// Handle dialog sound state change
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private async void DialogSoundState(object sender, MouseButtonEventArgs e)
        {
            var dialog = (DialogModel)((MenuItem)sender).DataContext;
            await ControllerBase.DialogController.EditDialog(
                dialog.Id,
                new DialogInEdit { IsSoundEnabled = !dialog.IsSoundEnabled }
            );
            dialog.IsSoundEnabled = !dialog.IsSoundEnabled;
            if (Current.MainWindow is not MainWindow window) return;
            window.ChatBoxListView.Items.Refresh();
        }

        /// <summary>
        /// Handle user block state change
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private async void UserBlockState(object sender, MouseButtonEventArgs e)
        {
            var userId = ((DialogModel)((MenuItem)sender).DataContext).User.Id;
            var result = await ControllerBase.UserController.BlockOrUnblockUser(userId);
            FlyMessenger.MainWindow.MainViewModel.MyProfile.BlackList = result.BlackList;
            FlyMessenger.MainWindow.MainViewModel.BlockedUsersCount = result.BlackList.Count;
            var dialog = FlyMessenger.MainWindow.MainViewModel.Dialogs.FirstOrDefault(d => d.User.Id == userId);
            if (dialog != null)
            {
                dialog.User.IsBlocked = result.IsBlocked;
                FlyMessenger.MainWindow.MainViewModel.DialogInputVisibility = !result.IsBlocked;
            }
            if (Current.MainWindow is not MainWindow window) return;
            window.ChatBoxListView.Items.Refresh();
        }

        /// <summary>
        /// Handle dialog delete
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private async void DialogDelete(object sender, MouseButtonEventArgs e)
        {
            if (Current.MainWindow is not MainWindow window) return;
            var listView = window.ChatBoxListView;
            await ControllerBase.DialogController.DeleteDialog(((DialogModel)((MenuItem)sender).DataContext).Id);
            FlyMessenger.MainWindow.MainViewModel.Dialogs.Remove((DialogModel)((MenuItem)sender).DataContext);
            listView.Items.Refresh();
        }

        /// <summary>
        /// Method for opening gallery window
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
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
