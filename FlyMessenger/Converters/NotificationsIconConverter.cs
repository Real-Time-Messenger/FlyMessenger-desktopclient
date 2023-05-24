using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FlyMessenger.Converters
{
    /// <summary>
    /// Notifications icon converter.
    /// </summary>
    public class NotificationsIconConverter : IValueConverter
    {
        // Convert notifications status to icon.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is true
                ? Application.Current.FindResource("Bell")
                : Application.Current.FindResource("Dialog-Bell-Off"))!;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
