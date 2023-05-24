using System;
using System.Windows.Data;

namespace FlyMessenger.Converters
{
    /// <summary>
    /// First letter to upper converter.
    /// </summary>
    public class FistLetterToUpperConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Convert first letter of string to upper.
            var str = value as string;
            if (str == null)
                return null;
            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);
            return str.ToUpper();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
