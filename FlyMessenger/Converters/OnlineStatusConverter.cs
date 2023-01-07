using System;
using System.Windows.Data;

namespace FlyMessenger.Converters
{
    public class OnlineStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool status)
            {
                return status ? "Online" : "Offline";
            }
            return "Offline";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
