using System;
using System.Globalization;
using System.Windows.Data;

namespace FlyMessenger.Converters
{
    /// <summary>
    /// DateTime converter for messages.
    /// </summary>
    public class DateTimeConverter : IValueConverter
    {
        // Convert DateTime to string.
        public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? null : DateTime.Parse(value.ToString()!).ToString("HH:mm");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
