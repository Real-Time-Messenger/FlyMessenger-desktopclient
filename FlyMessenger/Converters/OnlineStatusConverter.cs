using System;
using System.Windows.Data;

namespace FlyMessenger.Converters
{
    /// <summary>
    /// Online status converter.
    /// </summary>
    public class OnlineStatusConverter : IValueConverter
    {
        // Convert online status to string.
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
