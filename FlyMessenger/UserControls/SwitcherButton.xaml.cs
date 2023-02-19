using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace FlyMessenger.UserControls
{
    public partial class SwitcherButton : UserControl
    {
        public SwitcherButton()
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
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(SwitcherButton));

        // Button background color
        public Color BackgroundColor
        {
            get => (Color)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }

        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register(nameof(BackgroundColor), typeof(Color), typeof(SwitcherButton));

        // Button icon
        public DrawingImage Icon
        {
            get => (DrawingImage)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(
                nameof(System.Drawing.Icon),
                typeof(DrawingImage),
                typeof(SwitcherButton)
            );
        
        public bool CheckState
        {
            get => (bool)GetValue(CheckStateProperty);
            set => SetValue(CheckStateProperty, value);
        }
        
        public static readonly DependencyProperty CheckStateProperty =
            DependencyProperty.Register(nameof(CheckState), typeof(bool), typeof(SwitcherButton));

        private void OnIsCheckedChanged(object sender, MouseButtonEventArgs e)
        {
            UcCheckBox.IsChecked = !UcCheckBox.IsChecked;
            
            if (UcCheckBox.IsChecked == true)
            {
                
            }
            else
            {
                UcCheckBox.Background = new SolidColorBrush(Colors.Black);
                UcCheckBox.Foreground = new SolidColorBrush(Colors.White);
            }
        }
    }
}
