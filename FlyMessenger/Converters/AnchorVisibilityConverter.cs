using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FlyMessenger.Converters
{
    /// <summary>
    /// Anchor Visibility Converter
    /// </summary>
    public class AnchorVisibilityConverter : IValueConverter
    {
        // Show or hide anchor
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
