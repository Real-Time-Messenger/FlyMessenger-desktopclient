using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FlyMessenger.Controllers
{
    public static class ImageController
    {
        public static ImageSource GetImageFromBytes(byte[] imageBytes)
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = new System.IO.MemoryStream(imageBytes);
            image.DecodePixelWidth = 100;
            image.EndInit();

            return image;
        }
    }
}
