using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FlyMessenger.MVVM.ViewModels;

namespace FlyMessenger.Converters
{
    /// <summary>
    /// Image to source converter.
    /// </summary>
    public class ImageToSourceConverter : IValueConverter
    {
        // Convert image to source.
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ImageSource sessionImage;

            // If session is desktop, then set desktop image, else set web image.
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
