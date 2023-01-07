using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using FlyMessenger.Core.Utils;
using FlyMessenger.MVVM.ViewModels;
using FlyMessenger.Properties;
using FlyMessenger.Resources.Settings;
using FlyMessenger.Resources.Settings.Pages;
using FlyMessenger.UserControls;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace FlyMessenger
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly NotifyIconManager _notifyIconManager = new NotifyIconManager();
        
        protected override void OnStartup(StartupEventArgs e)
        {
            
            var langCode = Settings.Default.LanguageCode;
            Thread.CurrentThread.CurrentUICulture = langCode != "" ? new CultureInfo(langCode) : new CultureInfo("");
            
            var dict = new ResourceDictionary
            {
                Source = new Uri("Resources/Colors/Light.xaml", UriKind.Relative)
            };
            Resources.MergedDictionaries.Add(dict);
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            
            _notifyIconManager.InitializeNotifyIcon();

            // if (FlyMessenger.Properties.Settings.Default.IsFirstRun)
            // {
            //     var firstRunWindow = new FirstRunWindow();
            //     firstRunWindow.ShowDialog();
            // }
            // else
            // {
            //     var loginWindow = new LoginWindow();
            //     loginWindow.ShowDialog();
            // }
        }

        public static void ToggleLanguage(object? sender, EventArgs e)
        {
            var window = (MainWindow) sender;
            if (window == null || string.IsNullOrEmpty(window.LangSwitch)) return;

            window.Closed -= ToggleLanguage;
            window.Close();

            var language = window.LangSwitch;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
            Thread.CurrentThread.CurrentCulture = new CultureInfo(language);
            Settings.Default.LanguageCode = language;
            Settings.Default.Save();

            window = new MainWindow();
            window.Show();
        }

        public App() : base()
        {
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(
                "Unhandled exception occurred: \n" + e.Exception,
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
            // Show more details in debug mode
            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Break();
            }
        }
    }
}
