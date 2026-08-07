using System;
using System.Windows;
using System.Windows.Controls;
using WarToolKit.Models;
using WarToolKit.Services;

namespace WarToolKit.Controls
{
    public partial class PreviewPanel : System.Windows.Controls.UserControl
    {
        private readonly BLPReader reader = new();

        public PreviewPanel()
        {
            InitializeComponent();
        }

        public void Display(TextureFile texture)
        {
            FileNameText.Text =
                texture.Name;

            SizeText.Text =
                $"Size: {texture.FormattedFileSize}";

            PreviewImage.Source = null;

            try
            {
                // -----------------------------------------
                // Read BLP header
                // -----------------------------------------

                var info =
                    reader.ReadHeader(texture.FullPath);

                ResolutionText.Text =
                    $"Resolution: {info.Width} x {info.Height}";

                FormatText.Text =
                    $"Format: {info.Magic} ({info.Compression})";

                // -----------------------------------------
                // Read first mipmap
                // -----------------------------------------

                if (info.Compression == BLPCompression.JPEG)
                {
                    var mip =
                        reader.ReadFirstMip(texture.FullPath);

                    var image =
                        ImageDecoder.DecodeJpeg(mip);

                    PreviewImage.Source =
                        image;
                }
                else if (info.Compression == BLPCompression.Palette)
                {
                    var paletteReader =
                        new BLP1Reader();

                    var rgba =
                        paletteReader.ReadPaletteMip(
                            texture.FullPath);

                    var image =
                        ImageDecoder.DecodePalette(
                            rgba,
                            info.Width,
                            info.Height);

                    PreviewImage.Source =
                        image;
                }
                else
                {
                    throw new NotSupportedException(
                        $"Unsupported BLP compression: {info.Compression}");
                }
            }
            catch (Exception ex)
            {
                ResolutionText.Text =
                    "Resolution: Unknown";

                FormatText.Text =
                    "Format: Invalid";

                PreviewImage.Source = null;

                System.Windows.MessageBox.Show(
                    ex.ToString(),
                    "BLP Preview Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}