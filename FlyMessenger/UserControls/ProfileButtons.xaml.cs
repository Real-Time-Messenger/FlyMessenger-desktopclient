using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using FlyMessenger.MVVM.ViewModels;
using FlyMessenger.Resources.Settings;

namespace FlyMessenger.UserControls
{
    public partial class ProfileButtons : UserControl
    {
        public ProfileButtons()
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
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(ProfileButtons));
        
        // Text Data for button
        public string TextData
        {
            get => (string)GetValue(TextDataProperty);
            set => SetValue(TextDataProperty, value);
        }
        
        public static readonly DependencyProperty TextDataProperty =
            DependencyProperty.Register(nameof(TextData), typeof(string), typeof(ProfileButtons));

        // Button background color
        public Color BackgroundColor
        {
            get => (Color)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }

        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register(nameof(BackgroundColor), typeof(Color), typeof(ProfileButtons));

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
                typeof(ProfileButtons)
            );
    }
}
