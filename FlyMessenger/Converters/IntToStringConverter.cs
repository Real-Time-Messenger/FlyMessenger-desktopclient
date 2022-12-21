using System;
using System.Windows.Data;

namespace FlyMessenger.Converters
{
    public class IntToStringConverter : IValueConverter
    {
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
