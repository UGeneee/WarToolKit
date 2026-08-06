using System.Windows.Controls;
using WarToolKit.Models;

namespace WarToolKit.Controls
{
    public partial class PreviewPanel : System.Windows.Controls.UserControl
    {
        public PreviewPanel()
        {
            InitializeComponent();
        }

        public void Display(TextureFile texture)
        {
            FileNameText.Text = texture.Name;
            SizeText.Text = $"Size: {texture.FormattedFileSize}";
            ResolutionText.Text = "Resolution: Unknown";

            PreviewImage.Source = null;
        }
    }
}