using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FlyMessenger.MVVM.ViewModels;

namespace FlyMessenger.Converters
{
    public class ImageToSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Check each MainViewModel.Session element
            ImageSource sessionImage;

            if (value.ToString() == "desktop")
            {
                sessionImage = Application.Current.Resources["Sessions-Desktop"] as ImageSource;
            }
            else
            {
                sessionImage = Application.Current.Resources["Sessions-Web"] as ImageSource;
            }
            return sessionImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
