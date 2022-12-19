using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FlyMessenger.UserControls
{
    /// <summary>
    /// Логика взаимодействия для ChatMenuButton.xaml
    /// </summary>
    public partial class ChatMenuButton : UserControl
    {
        public ChatMenuButton()
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
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(ChatMenuButton));

        // Path data for svg icon
        public string Data
        {
            get => (string)GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register(nameof(Data), typeof(string), typeof(ChatMenuButton));

        // Button background color
        public Color BackgroundColor
        {
            get => (Color)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }

        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register(nameof(BackgroundColor), typeof(Color), typeof(ChatMenuButton));
        
        // Button foreground color
        public Brush ForegroundColor
        {
            get => (Brush)GetValue(ForegroundColorProperty);
            set => SetValue(ForegroundColorProperty, value);
        }
        
        public static readonly DependencyProperty ForegroundColorProperty =
            DependencyProperty.Register(nameof(ForegroundColor), typeof(Brush), typeof(ChatMenuButton));
        
        // button stroke color
        public Brush StrokeColor
        {
            get => (Brush)GetValue(StrokeColorProperty);
            set => SetValue(StrokeColorProperty, value);
        }

        public static readonly DependencyProperty StrokeColorProperty =
            DependencyProperty.Register(nameof(StrokeColor), typeof(Brush), typeof(ChatMenuButton));

        // Check if button IsActive
        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register(nameof(IsActive), typeof(bool), typeof(ChatMenuButton));
    }
}
