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

        // =========================================================
        // DISPLAY BLP TEXTURE
        // =========================================================

        public void Display(TextureFile texture)
        {
            FileNameText.Text = texture.Name;

            SizeText.Text =
                $"Size: {texture.FormattedFileSize}";

            PreviewImage.Source = null;

            try
            {
                // Read BLP header
                var info =
                    reader.ReadHeader(texture.FullPath);

                // Display resolution
                ResolutionText.Text =
                    $"Resolution: {info.Width} x {info.Height}";

                // Display format
                FormatText.Text =
                    $"Format: BLP1 ({info.Compression})";

                // =================================================
                // JPEG-COMPRESSED BLP
                // =================================================

                if (info.Compression == BLPCompression.JPEG)
                {
                    var mip =
                        reader.ReadFirstMip(texture.FullPath);

                    var image =
                        ImageDecoder.DecodeJpeg(mip);

                    PreviewImage.Source = image;
                }

                // =================================================
                // PALETTE-COMPRESSED BLP
                // =================================================

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

                    PreviewImage.Source = image;
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
                    ex.Message,
                    "ERROR",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // =========================================================
        // DISPLAY MDX MODEL INFORMATION
        // =========================================================

        public void DisplayModel(MDXFile model)
        {
            // Model filename
            FileNameText.Text =
                model.Name;

            // Models don't have a normal file-size display yet
            SizeText.Text =
                "MDX Model";

            // We don't have a 3D renderer yet
            PreviewImage.Source = null;

            // Show MDX version
            ResolutionText.Text =
                $"Version: {model.Version}";

            // Show number of textures
            FormatText.Text =
                $"Textures: {model.Textures.Count}";
        }

        private void ModelTextureList_SelectionChanged(
    object sender,
    SelectionChangedEventArgs e)
        {
            if (ModelTextureList.SelectedItem is string texturePath)
            {
                System.Windows.MessageBox.Show(
                    texturePath,
                    "MDX Texture Reference");
            }
        }
    }
}