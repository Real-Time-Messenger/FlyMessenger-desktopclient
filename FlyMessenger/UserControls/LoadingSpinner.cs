using System.Windows;
using System.Windows.Controls;

namespace FlyMessenger.UserControls
{
    /// <summary>
    /// Logic for LoadingSpinner.xaml
    /// </summary>
    public class LoadingSpinner : Control
    {
        static LoadingSpinner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LoadingSpinner), new FrameworkPropertyMetadata(typeof(LoadingSpinner)));
        }
    }
}
