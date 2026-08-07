using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WarToolKit.Services
{
    public static class ImageDecoder
    {
        public static BitmapImage DecodeJpeg(byte[] data)
        {
            using var stream =
                new MemoryStream(data);

            var image =
                new BitmapImage();

            image.BeginInit();

            image.CacheOption =
                BitmapCacheOption.OnLoad;

            image.StreamSource =
                stream;

            image.EndInit();

            image.Freeze();

            return image;
        }

        public static BitmapSource DecodePalette(
        byte[] rgba,
        int width,
        int height)
        {
            return BitmapSource.Create(
                width,
                height,
                96,
                96,
                PixelFormats.Bgra32,
                null,
                rgba,
                width * 4);
        }
    }
}