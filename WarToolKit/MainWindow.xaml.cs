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

                FileCountText.Text = $"{files.Count} Files Found";
            }
        }
    }
}