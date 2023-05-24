using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using FlyMessenger.MVVM.Model;

namespace FlyMessenger.Resources.Windows
{
    /// <summary>
    /// Interaction logic for GalleryWindow.xaml
    /// </summary>
    public partial class GalleryWindow
    {
        private ObservableCollection<MessageModel?> GalleryFiles { get; set; } = new ObservableCollection<MessageModel?>();
        private DialogModel GalleryDialog { get; set; }
        private string? CurrentImage { get; set; }
        private int _currentIndex;
        private bool _isDragging;
        private bool _isClicked;
        private Point _lastPosition;

        /// <summary>
        /// Gallery window constructor
        /// </summary>
        /// <param name="dialog">Dialog model</param>
        /// <param name="file">Current image</param>
        public GalleryWindow(DialogModel dialog, string file)
        {
            DataContext = MainWindow.MainViewModel;
            GalleryDialog = dialog;
            CurrentImage = file;

            Initialized += WindowInitialized;
            InitializeComponent();

            // Set events for buttons
            BaseImage.MouseMove += ImageDrag;
            BaseImage.MouseUp += ImageStopDragging;
            
            KeyDown += (sender, args) =>
            {
                switch (args.Key)
                {
                    case Key.Escape:
                        Close();
                        break;
                    case Key.Right:
                        NextButtonClick(sender, args);
                        break;
                    case Key.Left:
                        PreviousButtonClick(sender, args);
                        break;
                }
            };
        }

        /// <summary>
        /// Initialize gallery window
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void WindowInitialized(object? sender, EventArgs e)
        {
            // Get all messages with file from dialog
            foreach (var message in GalleryDialog.Messages.Where(message => message.File != null))
            {
                GalleryFiles.Add(message);
            }

            // Get current image index
            _currentIndex = GalleryFiles.IndexOf(
                GalleryFiles.FirstOrDefault(
                    message => message?.File == CurrentImage
                )
            );

            // If _imagesCount is 0, hide previous button
            if (_currentIndex == 0)
                PreviousButton.Visibility = Visibility.Collapsed;

            // If _imagesCount is last, hide next button
            if (_currentIndex == GalleryFiles.Count - 1)
                NextButton.Visibility = Visibility.Collapsed;

            if (CurrentImage != null) ImageSource = new BitmapImage(new Uri(CurrentImage));
        }

        #region Getters and setters
        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register(nameof(ImageSource), typeof(ImageSource), typeof(GalleryWindow));

        public double Zoom
        {
            get => (double)GetValue(ZoomProperty);
            set => SetValue(ZoomProperty, value);
        }

        public static readonly DependencyProperty ZoomProperty =
            DependencyProperty.Register(nameof(Zoom), typeof(double), typeof(GalleryWindow), new PropertyMetadata(1.0));
        #endregion
        
        /// <summary>
        /// Handle close button click
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Handle next button click
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void NextButtonClick(object sender, RoutedEventArgs e)
        {
            if (_currentIndex == GalleryFiles.Count - 1) return;
            HideLoader(_currentIndex, e);
            var uriString = GalleryFiles[_currentIndex + 1]?.File;

            if (uriString != null)
            {
                ImageSource = new BitmapImage(
                    new Uri(
                        uriString
                    )
                );

                PreviousButton.Visibility = Visibility.Visible;
                _currentIndex++;
            }

            if (_currentIndex != GalleryFiles.Count - 1) return;
            NextButton.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Handle previous button click
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void PreviousButtonClick(object sender, RoutedEventArgs e)
        {
            if (_currentIndex == 0) return;
            HideLoader(_currentIndex, e);
            var uriString = GalleryFiles[_currentIndex - 1]?.File;

            if (uriString != null)
            {
                ImageSource = new BitmapImage(
                    new Uri(
                        uriString
                    )
                );

                NextButton.Visibility = Visibility.Visible;
                _currentIndex--;
            }

            if (_currentIndex != 0) return;
            PreviousButton.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Handle outside click
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void OutsideClick(object sender, MouseButtonEventArgs e)
        {
            if (e.Source.Equals(this))
            {
                _isClicked = true;
            }
        }

        /// <summary>
        /// Handle isClicked flag
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void OutsideClickHandled(object sender, MouseButtonEventArgs e)
        {
            if (!_isClicked) return;
            Close();
            _isClicked = false;
        }

        private void Image_Scale(object sender, MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            var currentScale = BaseImage.RenderTransform.Value.M11;
            
            // Zoom in/out
            switch (BaseImage.Source.Width > 2000, BaseImage.Source.Height > 2000, currentScale, e.Delta)
            {
                case (true, false, < 0.51, < 0):
                case (true, false, > 9.99, > 0):
                case (false, true, < 0.51, < 0):
                case (false, true, > 9.99, > 0):
                case (false, false, < 0.51, < 0):
                case (false, false, > 5.99, > 0):
                    return;
            }

            BaseImage.RenderTransform = e.Delta switch
            {
                > 0 =>
                    new ScaleTransform(currentScale + 0.25, currentScale + 0.25),
                < 0 =>
                    new ScaleTransform(currentScale - 0.25, currentScale - 0.25),
                _ => BaseImage.RenderTransform
            };

            Zoom = BaseImage.RenderTransform.Value.M11;
        }

        /// <summary>
        /// Handle image move
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void ImageMove(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || !IsImageOutOfBounds()) return;
            _isDragging = true;
            _lastPosition = e.GetPosition(BaseImage);
        }

        /// <summary>
        /// Method to drag image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageDrag(object sender, MouseEventArgs e)
        {
            if (!_isDragging) return;

            var currentPosition = e.GetPosition(BaseImage);
            var currentWindow = new Rect(Left, Top, Width, Height);
            var currentImage = BaseImage.TransformToAncestor(this).TransformBounds(new Rect(BaseImage.RenderSize));

            // Calculate new X and Y
            var newX = (currentPosition.X - _lastPosition.X);
            var newY = (currentPosition.Y - _lastPosition.Y);

            // Check if image is out of bounds
            if (currentImage.Left + newX > currentWindow.Left + 8)
                newX = 0;

            if (currentImage.Right + newX < currentWindow.Right)
                newX = 0;

            if (currentImage.Top + newY > currentWindow.Top + 8)
                newY = 0;

            if (currentImage.Bottom + newY < currentWindow.Bottom)
                newY = 0;

            // Move image
            BaseImage.RenderTransform = new MatrixTransform(
                BaseImage.RenderTransform.Value.M11,
                BaseImage.RenderTransform.Value.M12,
                BaseImage.RenderTransform.Value.M21,
                BaseImage.RenderTransform.Value.M22,
                BaseImage.RenderTransform.Value.OffsetX + newX,
                BaseImage.RenderTransform.Value.OffsetY + newY
            );

            _lastPosition = currentPosition;
        }

        /// <summary>
        /// Handle image stop dragging
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void ImageStopDragging(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
        }

        /// <summary>
        /// Handle bounds of image
        /// </summary>
        /// <returns>True if image is out of bounds</returns>
        private bool IsImageOutOfBounds()
        {
            var imageBounds = BaseImage.TransformToAncestor(this).TransformBounds(new Rect(BaseImage.RenderSize));
            var windowBounds = new Rect(0, 0, ActualWidth, ActualHeight);

            return !windowBounds.Contains(imageBounds);
        }

        /// <summary>
        /// Method to hide loader
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void HideLoader(object sender, RoutedEventArgs e)
        {
            LoadingSpinner.Opacity = 1;
            LoadingSpinner.Visibility = Visibility.Visible;
            
            var animation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(10))
            };

            animation.Completed += (_, _) => { LoadingSpinner.Visibility = Visibility.Collapsed; };

            LoadingSpinner.BeginAnimation(OpacityProperty, animation);
        }
    }
}
