using System.Windows;
using System.Windows.Media;

namespace FlyMessenger.UserControls
{
    /// <summary>
    /// Logic for SecondSettingsButton.xaml
    /// </summary>
    public partial class SecondSettingsButton
    {
        public SecondSettingsButton()
        {
            InitializeComponent();
        }

        // Title for button
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(SecondSettingsButton));

        // Button background color
        public Color BackgroundColor
        {
            get => (Color)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }

        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register(nameof(BackgroundColor), typeof(Color), typeof(SecondSettingsButton));

        // Button foreground color
        public Brush ForegroundColor
        {
            get => (Brush)GetValue(ForegroundColorProperty);
            set => SetValue(ForegroundColorProperty, value);
        }

        public static readonly DependencyProperty ForegroundColorProperty =
            DependencyProperty.Register(nameof(ForegroundColor), typeof(Brush), typeof(SecondSettingsButton));

        // Button icon
        public DrawingImage Icon
        {
            get => (DrawingImage)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(DrawingImage), typeof(SecondSettingsButton));
    }
}
