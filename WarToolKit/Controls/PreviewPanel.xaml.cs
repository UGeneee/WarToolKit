using System.Windows.Controls;
using WarToolKit.Models;
using WarToolKit.Services;

namespace WarToolKit.Controls
{

    public partial class PreviewPanel : System.Windows.Controls.UserControl
    {
        public PreviewPanel()
        {
            InitializeComponent();
        }

        private readonly BLPReader reader = new();

        public void Display(TextureFile texture)
        {
            FileNameText.Text = texture.Name;

            SizeText.Text = $"Size: {texture.FormattedFileSize}";

            PreviewImage.Source = null;

            try
            {
                var info = reader.Read(texture.FullPath);

                ResolutionText.Text =
                    $"Resolution: {info.Width} x {info.Height}";

                FormatText.Text =
                    $"Format: {info.Version} ({info.Compression})";
            }
            catch
            {
                ResolutionText.Text = "Resolution: Unknown";

                FormatText.Text = "Format: Invalid";
            }
        }
    }
}