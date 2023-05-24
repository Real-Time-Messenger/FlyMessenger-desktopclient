using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FlyMessenger.Controllers
{
    /// <summary>
    /// This class is used to convert byte array to ImageSource.
    /// </summary>
    public static class ImageController
    {
        // Convert byte array to ImageSource.
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
