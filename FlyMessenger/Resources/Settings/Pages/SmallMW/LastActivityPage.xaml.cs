using System.Windows;
using System.Windows.Controls;
using FlyMessenger.Controllers;
using FlyMessenger.Resources.Languages;

namespace FlyMessenger.Resources.Settings.Pages.SmallMW
{
    /// <summary>
    /// Interaction logic for LastActivityPage.xaml
    /// </summary>
    public partial class LastActivityPage : Page
    {
        public LastActivityPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handler for anyone radio button click.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private async void OnAnyoneRadioButtonClick(object sender, RoutedEventArgs e)
        {
            await ControllerBase.UserController.EditMyLastActivity(true);
            App.LastActivityTextData.TextData = lang.anyone;
        }

        /// <summary>
        /// Handler for nobody radio button click.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private async void OnNobodyRadioButtonClick(object sender, RoutedEventArgs e)
        {
            await ControllerBase.UserController.EditMyLastActivity(false);
            App.LastActivityTextData.TextData = lang.nobody;
        }
    }
}

