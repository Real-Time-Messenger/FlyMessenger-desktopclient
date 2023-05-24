using System;
using System.Globalization;
using System.Windows.Data;

namespace FlyMessenger.Converters
{
    /// <summary>
    /// Date to date converter
    /// </summary>
    public class DateToDateConverter : IValueConverter
    {
        /// <summary>
        /// Converts a given value to a date string representation.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The type of the target property.</param>
        /// <param name="parameter">An optional parameter.</param>
        /// <param name="culture">The culture used for formatting.</param>
        /// <returns>A formatted date string representation of the given value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value.ToString()))
                return DateTime.Now;

            // if value is current date, return "Today"
            if (DateTime.Parse(value.ToString()!).Date == DateTime.Now.Date)
                return Resources.Languages.lang.today;
            
            // if value is yesterday, return "Yesterday"
            if (DateTime.Parse(value.ToString()!).Date == DateTime.Now.Date.AddDays(-1))
                return Resources.Languages.lang.yesterday;
            
            // if value is now year, return "dd MMMM", else return "dd.MM.yyyy"
            return DateTime.Parse(value.ToString()!).Year == DateTime.Now.Year
                ? DateTime.Parse(value.ToString()!).ToString("dd MMMM")
                : DateTime.Parse(value.ToString()!).ToString("dd.MM.yyyy");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
