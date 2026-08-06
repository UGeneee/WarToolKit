using System.Windows;
using WarToolKit.Services;

namespace WarToolKit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly FileScanner scanner = new();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            using FolderBrowserDialog dialog = new();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var files = scanner.ScanFolder(dialog.SelectedPath);

                FileList.ItemsSource = files;

                StatusText.Text = "Folder loaded successfully.";
                FolderPathText.Text = dialog.SelectedPath;

                FolderFileCount.Text = $"{files.Count} texture files";
            }
        }

        private void FileList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (FileList.SelectedItem is Models.TextureFile texture)
            {
                PreviewPanel.Display(texture);
            }
        }

    }
}