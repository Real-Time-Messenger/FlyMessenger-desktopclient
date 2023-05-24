using System;
using System.Globalization;
using System.Windows.Data;

namespace FlyMessenger.Converters
{
    /// <summary>
    /// Width converter.
    /// </summary>
    public class WidthConverter : IValueConverter
    {
        // Convert width to double.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value - 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
