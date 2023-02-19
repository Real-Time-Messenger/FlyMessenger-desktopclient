using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using FlyMessenger.Controllers;
using Microsoft.Win32;
using RestSharp;

namespace FlyMessenger.Resources.Settings.Pages
{
    public partial class ProfilePage : Page
    {
        public ProfilePage()
        {
            InitializeComponent();
        }

        private void OpenNameEditModalWindow(object sender, MouseButtonEventArgs e)
        {
            if (Application.Current.MainWindow is not MainWindow window) return;
            window.NameEditModalWindow.IsOpen = true;
            
            var openAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.1));
            window.NameEditModalWindow.BeginAnimation(OpacityProperty, openAnimation);
        }

        private void OpenEmailEditModalWindow(object sender, MouseButtonEventArgs e)
        {
            var window = (MainWindow?)Application.Current.MainWindow;
            window!.EmailEditModalWindow.IsOpen = true;
            
            var openAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.1));
            window.EmailEditModalWindow.BeginAnimation(OpacityProperty, openAnimation);
        }

        private void CopyUsernameToClipboard(object sender, MouseButtonEventArgs e)
        {
            if (Application.Current.MainWindow is not MainWindow window || window.UsernameCopiedTip.IsOpen) return;
            
            window.UsernameCopiedTip.IsOpen = true;
            
            Clipboard.SetText(UsernameButton.TextData);

            var openAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.2));
            var closeAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.2))
            {
                BeginTime = TimeSpan.FromSeconds(2)
            };
            
            openAnimation.Completed += (_, _) => window.UsernameCopiedTip.BeginAnimation(OpacityProperty, closeAnimation);
            closeAnimation.Completed += (_, _) => window.UsernameCopiedTip.IsOpen = false;
            
            window.UsernameCopiedTip.BeginAnimation(OpacityProperty, openAnimation);
        }

        private void ChangeProfilePhoto(object sender, MouseButtonEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg;*.jpeg;)|*.png;*.jpg;*.jpeg;",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                Multiselect = false,
                Title = "Select a profile photo"
            };
            
            if (openFileDialog.ShowDialog() != true) return;
            var fileToBytes = File.ReadAllBytes(openFileDialog.FileName);

            ControllerBase.UserController.EditMyProfilePhoto(fileToBytes);
            
            ProfilePhotoProfilePage.ImageSource = ImageController.GetImageFromBytes(fileToBytes);
            App.ProfilePhotoDefaultPage.ImageSource = ImageController.GetImageFromBytes(fileToBytes);
            App.ProfilePhotoMainWindow.ImageSource = ImageController.GetImageFromBytes(fileToBytes);
        }
    }
}

