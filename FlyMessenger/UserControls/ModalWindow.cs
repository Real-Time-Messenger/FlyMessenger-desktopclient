using System.Windows;
using System.Windows.Controls;

namespace FlyMessenger.UserControls
{
    public class ModalWindow : ContentControl
    {
        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }
        
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(ModalWindow), new PropertyMetadata(false));
    }
}
