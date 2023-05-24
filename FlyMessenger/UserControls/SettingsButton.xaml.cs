using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using FlyMessenger.Resources.Settings;

namespace FlyMessenger.UserControls
{
    /// <summary>
    /// Logic for SettingsButton.xaml
    /// </summary>
    public partial class SettingsButton : UserControl
    {
        public SettingsButton()
        {
            InitializeComponent();
        }
        
        // Id for button
        public int Id { 
            get => (int)GetValue(IdProperty);
            set => SetValue(IdProperty, value);
        }
        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register(nameof(Id), typeof(int), typeof(SettingsButton), new PropertyMetadata(0));

        // Title for button
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(SettingsButton));

        // Button background color
        public Color BackgroundColor
        {
            get => (Color)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }

        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register(nameof(BackgroundColor), typeof(Color), typeof(SettingsButton));

        // Button foreground color
        public Brush ForegroundColor
        {
            get => (Brush)GetValue(ForegroundColorProperty);
            set => SetValue(ForegroundColorProperty, value);
        }

        public static readonly DependencyProperty ForegroundColorProperty =
            DependencyProperty.Register(nameof(ForegroundColor), typeof(Brush), typeof(SettingsButton));

        // Button icon
        public DrawingImage Icon
        {
            get => (DrawingImage)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
        
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(
                nameof(Icon),
                typeof(DrawingImage),
                typeof(SettingsButton)
            );

        // Previous Menu Id Button
        public int? PreviousMenuId
        {
            get => (int)GetValue(PreviousMenuIdProperty);
            set => SetValue(PreviousMenuIdProperty, value);
        }
        public static readonly DependencyProperty PreviousMenuIdProperty =
            DependencyProperty.Register(nameof(PreviousMenuId), typeof(int?), typeof(SettingsButton));
        
        private void OpenPage(object sender, MouseButtonEventArgs e)
        {
            SettingsManager.OpenPage(Id);
        }
    }
}
