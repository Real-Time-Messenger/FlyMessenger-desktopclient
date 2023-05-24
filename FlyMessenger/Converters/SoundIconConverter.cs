using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FlyMessenger.Converters
{
    /// <summary>
    /// Sound icon converter.
    /// </summary>
    public class SoundIconConverter : IValueConverter
    {
        // Convert sound status to icon.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is true
                ? Application.Current.FindResource("Dialog-Volume-On")
                : Application.Current.FindResource("Dialog-Volume-Off"))!;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
