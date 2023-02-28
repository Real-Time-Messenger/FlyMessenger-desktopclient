using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using FlyMessenger.Controllers;

namespace FlyMessenger.Resources.Settings.Pages.SmallMW
{
    public partial class NameEditPage
    {
        public NameEditPage()
        {
            InitializeComponent();
        }

        private void NameSetActive(object sender, MouseButtonEventArgs e)
        {
            // Set animation
            var animation = new DoubleAnimation(NameEditLabel.Opacity, 1, TimeSpan.FromSeconds(0.1));
            var animationBorder = new ThicknessAnimation(
                new Thickness(0, 0, 0, NameEditTextBoxBorder.BorderThickness.Bottom),
                new Thickness(0, 0, 0, 2),
                TimeSpan.FromSeconds(0.1)
            );
            var animationExit = new DoubleAnimation(SurnameEditLabel.Opacity, 0.5, TimeSpan.FromSeconds(0.1));
            var animationBorderExit = new ThicknessAnimation(
                new Thickness(0, 0, 0, SurnameEditTextBoxBorder.BorderThickness.Bottom),
                new Thickness(0, 0, 0, 1),
                TimeSpan.FromSeconds(0.1)
            );

            // Start animation
            NameEditLabel.BeginAnimation(OpacityProperty, animation);
            NameEditTextBoxBorder.BeginAnimation(Control.BorderThicknessProperty, animationBorder);
            SurnameEditLabel.BeginAnimation(OpacityProperty, animationExit);
            SurnameEditTextBoxBorder.BeginAnimation(Control.BorderThicknessProperty, animationBorderExit);
        }

        private void LastNameSetActive(object sender, MouseButtonEventArgs e)
        {
            // Set animation
            var animation = new DoubleAnimation(SurnameEditLabel.Opacity, 1, TimeSpan.FromSeconds(0.1));
            var animationBorder = new ThicknessAnimation(
                new Thickness(0, 0, 0, SurnameEditTextBoxBorder.BorderThickness.Bottom),
                new Thickness(0, 0, 0, 2),
                TimeSpan.FromSeconds(0.1)
            );
            var animationExit = new DoubleAnimation(NameEditLabel.Opacity, 0.5, TimeSpan.FromSeconds(0.1));
            var animationBorderExit = new ThicknessAnimation(
                new Thickness(0, 0, 0, NameEditTextBoxBorder.BorderThickness.Bottom),
                new Thickness(0, 0, 0, 1),
                TimeSpan.FromSeconds(0.1)
            );

            // Start animation
            SurnameEditLabel.BeginAnimation(OpacityProperty, animation);
            SurnameEditTextBoxBorder.BeginAnimation(Control.BorderThicknessProperty, animationBorder);
            NameEditLabel.BeginAnimation(OpacityProperty, animationExit);
            NameEditTextBoxBorder.BeginAnimation(Control.BorderThicknessProperty, animationBorderExit);
        }

        private void OnNameEditCancelClick(object sender, RoutedEventArgs e)
        {
            var closeAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.1));
            if (Application.Current.MainWindow is not MainWindow mainWindow) return;

            closeAnimation.Completed += (_, _) => { mainWindow.NameEditModalWindow.IsOpen = false; };

            mainWindow.NameEditModalWindow.BeginAnimation(OpacityProperty, closeAnimation);
        }

        private void OnNameEditSaveClick(object sender, RoutedEventArgs e)
        {
            var name = NameEditTextBox.Text;
            var surname = SurnameEditTextBox.Text;
            
            NameErrorLabel.Visibility =
                IsValidLength(name, 3, 25) ? Visibility.Collapsed : Visibility.Visible;
            LastNameErrorLabel.Visibility =
                IsValidLength(surname, 3, 25) ? Visibility.Collapsed : Visibility.Visible;
            
            if (!IsValidLength(name, 3, 25) || !IsValidLength(surname, 3, 25)) return;
            ControllerBase.UserController.EditMyProfileName(name, surname);
        }
        
        private static bool IsValidLength(string text, int min, int max)
        {
            return text.Length >= min && text.Length <= max;
        }
    }
}
