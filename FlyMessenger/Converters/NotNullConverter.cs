using System;
using System.Globalization;
using System.Windows.Data;

namespace FlyMessenger.Converters
{
    /// <summary>
    /// Not null converter.
    /// </summary>
    public class NotNullConverter : IValueConverter
    {
        // Convert not null value to boolean.
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
