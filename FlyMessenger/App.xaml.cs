using System;
using System.Data;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace FlyMessenger
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var langCode = FlyMessenger.Properties.Settings.Default.LanguageCode;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(langCode);
            
            var dict = new ResourceDictionary
            {
                Source = new Uri("Resources/Colors/Light.xaml", UriKind.Relative)
            };
            Resources.MergedDictionaries.Add(dict);

            base.OnStartup(e);
        }
        
        public App() : base() {
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
            MessageBox.Show("Unhandled exception occurred: \n" + e.Exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            MessageBox.Show("Unhandled exception occurred: \n" + e.Exception.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
