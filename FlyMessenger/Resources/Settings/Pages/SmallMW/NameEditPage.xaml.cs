using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using FlyMessenger.Controllers;

namespace FlyMessenger.Resources.Settings.Pages.SmallMW
{
    /// <summary>
    /// Interaction logic for NameEditPage.xaml
    /// </summary>
    public partial class NameEditPage
    {
        public NameEditPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Visual effects for name edit label.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
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

        /// <summary>
        /// Visual effects for surname edit label.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
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

        /// <summary>
        /// Handler for name edit cancel button click.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void OnNameEditCancelClick(object sender, RoutedEventArgs e)
        {
            var closeAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.1));
            if (Application.Current.MainWindow is not MainWindow mainWindow) return;

            closeAnimation.Completed += (_, _) => { mainWindow.NameEditModalWindow.IsOpen = false; };

            mainWindow.NameEditModalWindow.BeginAnimation(OpacityProperty, closeAnimation);
        }

        /// <summary>
        /// Handler for name edit save button click.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private async void OnNameEditSaveClick(object sender, RoutedEventArgs e)
        {
            var name = NameEditTextBox.Text;
            var surname = SurnameEditTextBox.Text;
            
            NameErrorLabel.Visibility =
                IsValidLength(name, 3, 25) ? Visibility.Collapsed : Visibility.Visible;
            
            if (!IsValidLength(name, 3, 25)) return;
            await ControllerBase.UserController.EditMyProfileName(name, surname);
        }
        
        /// <summary>
        /// Validate text length.
        /// </summary>
        /// <param name="text">Text to validate.</param>
        /// <param name="min">Minimum length.</param>
        /// <param name="max">Maximum length.</param>
        /// <returns>True if text length is valid.</returns>
        private static bool IsValidLength(string text, int min, int max)
        {
            return text.Length >= min && text.Length <= max;
        }
    }
}
