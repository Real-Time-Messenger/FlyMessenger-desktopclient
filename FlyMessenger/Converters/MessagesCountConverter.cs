using System;
using System.Globalization;
using System.Windows.Data;

namespace FlyMessenger.Converters
{
    public class MessagesCountConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            // return string to int if value is not null and greater than 0
            if (value != null && int.Parse(value.ToString() ?? string.Empty) > 0)
            {
                return int.Parse(value.ToString() ?? string.Empty) > 0;
            }
            else
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
