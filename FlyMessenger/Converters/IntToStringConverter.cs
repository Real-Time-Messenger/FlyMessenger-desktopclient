using System;
using System.Windows.Data;

namespace FlyMessenger.Converters
{
    /// <summary>
    /// Integer to string converter.
    /// </summary>
    public class IntToStringConverter : IValueConverter
    {
        // Convert integer to string.
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int result;
            if (int.TryParse(value.ToString(), out result))
                return result;
            else
                return 0;
        }
    }
}
