using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FlyMessenger.UserControls
{
    /// <summary>
    /// Логика взаимодействия для TopBarButton.xaml
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
