using System;
using System.Windows;
using System.Windows.Data;

namespace FlyMessenger.Converters
{
    public class ChatBoxNameConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // If value = UnsetValue, then the binding is not ready yet.
            if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
                return null!;
            
            // Return two strings concatenated together
            return values[0] + " " + values[1];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
