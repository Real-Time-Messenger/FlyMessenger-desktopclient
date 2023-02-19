using System;
using System.Globalization;
using System.Windows.Data;

namespace FlyMessenger.Converters
{
    public class PinConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is true
                ? Resources.Languages.lang.unpin
                : Resources.Languages.lang.pin;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
