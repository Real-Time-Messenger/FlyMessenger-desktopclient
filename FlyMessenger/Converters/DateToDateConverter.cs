using System;
using System.Globalization;
using System.Windows.Data;
using FlyMessenger.Properties;

namespace FlyMessenger.Converters
{
    public class DateToDateConverter : IValueConverter
    {
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
