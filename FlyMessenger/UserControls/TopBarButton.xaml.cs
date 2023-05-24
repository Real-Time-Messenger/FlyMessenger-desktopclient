using System.Windows;
using System.Windows.Controls;

namespace FlyMessenger.UserControls
{
    /// <summary>
    /// Logic for TopBarButton.xaml
    /// </summary>
    public partial class TopBarButton : UserControl
    {
        public TopBarButton()
        {
            InitializeComponent();
        }

        // Path data for svg icon
        public string TopBarData
        {
            get => (string)GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register(nameof(TopBarData), typeof(string), typeof(TopBarButton));
    }
}
