using System;
using System.Globalization;
using System.Windows.Data;

namespace FlyMessenger.Converters
{
    public class SoundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is true
                ? Resources.Languages.lang.sound_off
                : Resources.Languages.lang.sound_on;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
