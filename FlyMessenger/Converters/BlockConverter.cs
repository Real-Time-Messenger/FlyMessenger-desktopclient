using System.Windows.Data;

namespace FlyMessenger.Converters
{
    public class BlockConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value is true
                ? Resources.Languages.lang.unblock_user
                : Resources.Languages.lang.block_user;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
