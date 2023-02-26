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
    public partial class GalleryWindow
    {
        private ObservableCollection<MessageModel?> GalleryFiles { get; set; } = new ObservableCollection<MessageModel?>();
        private DialogModel GalleryDialog { get; set; }
        private string? CurrentImage { get; set; }
        private int _currentIndex;
        private bool _isDragging;
        private bool _isClicked;
        private Point _lastPosition;

        public GalleryWindow(DialogModel dialog, string file)
        {
            DataContext = MainWindow.MainViewModel;
            GalleryDialog = dialog;
            CurrentImage = file;

            Initialized += Window_Initialized;
            InitializeComponent();

            BaseImage.MouseMove += Image_Drag;
            BaseImage.MouseUp += Image_StopDragging;
            
            KeyDown += (sender, args) =>
            {
                switch (args.Key)
                {
                    case Key.Escape:
                        Close();
                        break;
                    case Key.Right:
                        NextButton_Click(sender, args);
                        break;
                    case Key.Left:
                        PreviousButton_Click(sender, args);
                        break;
                }
            };
        }

        private void Window_Initialized(object? sender, EventArgs e)
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

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentIndex == GalleryFiles.Count - 1) return;
            Hide_Loader(_currentIndex, e);
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

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentIndex == 0) return;
            Hide_Loader(_currentIndex, e);
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

        private void Outside_Click(object sender, MouseButtonEventArgs e)
        {
            if (e.Source.Equals(this))
            {
                _isClicked = true;
            }
        }

        private void Outside_Click_Handled(object sender, MouseButtonEventArgs e)
        {
            if (!_isClicked) return;
            Close();
            _isClicked = false;
        }

        private void Image_Scale(object sender, MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            var currentScale = BaseImage.RenderTransform.Value.M11;

            if (BaseImage.Source.Width > 2000 && currentScale < 0.51 && e.Delta < 0 ||
                BaseImage.Source.Width > 2000 && currentScale > 9.99 && e.Delta > 0 ||
                BaseImage.Source.Height > 2000 && currentScale < 0.51 && e.Delta < 0 ||
                BaseImage.Source.Width > 2000 && currentScale > 9.99 && e.Delta > 0 ||
                BaseImage.Source.Width < 2000 && currentScale < 0.51 && e.Delta < 0 ||
                BaseImage.Source.Width < 2000 && currentScale > 5.99 && e.Delta > 0 ||
                BaseImage.Source.Height < 2000 && currentScale < 0.51 && e.Delta < 0 ||
                BaseImage.Source.Width < 2000 && currentScale > 5.99 && e.Delta > 0) return;

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

        private void Image_Move(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || !IsImageOutOfBounds()) return;
            _isDragging = true;
            _lastPosition = e.GetPosition(BaseImage);
        }

        private void Image_Drag(object sender, MouseEventArgs e)
        {
            // Если изображение не перемещается, то выходим
            if (!_isDragging) return;

            // Получаем текущую позицию изображения
            var currentPosition = e.GetPosition(BaseImage);

            // Получаем текущие границы окна
            var currentWindow = new Rect(Left, Top, Width, Height);

            // Получаем текущие границы изображения
            var currentImage = BaseImage.TransformToAncestor(this).TransformBounds(new Rect(BaseImage.RenderSize));

            // Получаем смещение
            var newX = (currentPosition.X - _lastPosition.X);
            var newY = (currentPosition.Y - _lastPosition.Y);

            // Разрешаем перемещение только тогда, когда изображение выходит за границы окна
            if (currentImage.Left + newX > currentWindow.Left + 8)
                newX = 0;

            if (currentImage.Right + newX < currentWindow.Right)
                newX = 0;

            if (currentImage.Top + newY > currentWindow.Top + 8)
                newY = 0;

            if (currentImage.Bottom + newY < currentWindow.Bottom)
                newY = 0;

            // Перемещаем изображение по координатам
            BaseImage.RenderTransform = new MatrixTransform(
                BaseImage.RenderTransform.Value.M11,
                BaseImage.RenderTransform.Value.M12,
                BaseImage.RenderTransform.Value.M21,
                BaseImage.RenderTransform.Value.M22,
                BaseImage.RenderTransform.Value.OffsetX + newX,
                BaseImage.RenderTransform.Value.OffsetY + newY
            );

            // Обновляем позицию курсора
            _lastPosition = currentPosition;
        }

        private void Image_StopDragging(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
        }

        private bool IsImageOutOfBounds()
        {
            var imageBounds = BaseImage.TransformToAncestor(this).TransformBounds(new Rect(BaseImage.RenderSize));
            var windowBounds = new Rect(0, 0, ActualWidth, ActualHeight);

            return !windowBounds.Contains(imageBounds);
        }

        private void Hide_Loader(object sender, RoutedEventArgs e)
        {
            LoadingSpinner.Opacity = 1;
            LoadingSpinner.Visibility = Visibility.Visible;
            
            var animation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(5))
            };

            animation.Completed += (_, _) => { LoadingSpinner.Visibility = Visibility.Collapsed; };

            LoadingSpinner.BeginAnimation(OpacityProperty, animation);
        }
    }
}
