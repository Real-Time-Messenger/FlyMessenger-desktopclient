using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FlyMessenger.Converters
{
    /// <summary>
    /// Last activity converter.
    /// </summary>
    public class LastActivityConverter : IValueConverter
    {
        // Convert last activity to string.
        public object Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            // if value is null or empty
            if (value == null)
                return Resources.Languages.lang.last_seen + " " + Resources.Languages.lang.long_time_ago;
            
            // if date like 02/14/2023 15:34:24, convert to 2023-02-14 15:34:24
            if (value.ToString()!.Contains('/'))
            {
                DateTime.TryParseExact(value.ToString(), "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date);
                value = date.ToString("yyyy-MM-dd HH:mm:ss");
            }
            
            // Get time difference 
            var timeDifference = DateTime.Now - DateTime.Parse(value.ToString()!);
            var timeDifferenceMinutes = timeDifference.Minutes;
            var timeDifferenceHours = timeDifference.Hours;
            var timeDifferenceDays = timeDifference.Days;

            // Get strings
            var minutesAgo = Resources.Languages.lang.last_seen + " " + timeDifferenceMinutes + " " +
                             Resources.Languages.lang.minutes_ago;
            var hourAgo = Resources.Languages.lang.last_seen + " " + Resources.Languages.lang.hour_ago;
            var hoursAgo = Resources.Languages.lang.last_seen + " " + timeDifferenceHours + " " +
                           Resources.Languages.lang.hours_ago;
            var yesterday = Resources.Languages.lang.last_seen + " " + Resources.Languages.lang.yesterday;
            var thisWeek = Resources.Languages.lang.last_seen + " " + Resources.Languages.lang.this_week;
            var recently = Resources.Languages.lang.last_seen + " " + Resources.Languages.lang.recently;
            var longTimeAgo = Resources.Languages.lang.last_seen + " " + Resources.Languages.lang.long_time_ago;

            // Return string
            return timeDifferenceDays switch
            {
                0 when timeDifferenceMinutes <= 1 => Resources.Languages.lang.last_seen + " " + 1 + " " +
                                                     Resources.Languages.lang.minutes_ago,
                0 when timeDifferenceMinutes is > 1 and < 60 => minutesAgo,
                0 when timeDifferenceHours < 2 => hourAgo,
                0 when timeDifferenceHours is < 24 and > 1 => hoursAgo,
                1 => yesterday,
                2 => thisWeek,
                >= 3 and < 7 => recently,
                _ => longTimeAgo
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
