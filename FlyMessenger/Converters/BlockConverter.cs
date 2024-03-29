using System.Windows.Data;

namespace FlyMessenger.Converters
{
    /// <summary>
    /// Block Converter
    /// </summary>
    public class BlockConverter : IValueConverter
    {
        // Block or unblock user
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
