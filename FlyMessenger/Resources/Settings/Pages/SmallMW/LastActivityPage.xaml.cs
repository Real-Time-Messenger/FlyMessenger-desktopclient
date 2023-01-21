using System.Windows;
using System.Windows.Controls;
using FlyMessenger.Controllers;
using FlyMessenger.MVVM.ViewModels;
using FlyMessenger.Resources.Languages;

namespace FlyMessenger.Resources.Settings.Pages.SmallMW
{
    public partial class LastActivityPage : Page
    {
        public LastActivityPage()
        {
            InitializeComponent();
        }

        private void OnAnyoneRadioButtonClick(object sender, RoutedEventArgs e)
        {
            ControllerBase.UserController.EditMyLastActivity(true);
            App.LastActivityTextData.TextData = lang.anyone;
        }

        private void OnNobodyRadioButtonClick(object sender, RoutedEventArgs e)
        {
            ControllerBase.UserController.EditMyLastActivity(false);
            App.LastActivityTextData.TextData = lang.nobody;
        }
    }
}

