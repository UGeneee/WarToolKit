using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using WarToolKit.Models;
using WarToolKit.Parsers;
using WarToolKit.Services;

using DataFormats = System.Windows.DataFormats;

namespace WarToolKit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // =========================================================
        // FILE LISTS
        // =========================================================

        private List<TextureFile> allFiles = new();

        private List<TextureFile> filteredFiles = new();

        private List<MDXFile> modelFiles = new();

        private List<ExtractedBLPInfo> extractedBLPs = new();


        // =========================================================
        // SERVICES
        // =========================================================

        private readonly MDXParser mdxParser = new();

        private readonly FileScanner scanner = new();


        // =========================================================
        // SORTING
        // =========================================================

        private GridViewColumnHeader? lastHeaderClicked;

        private ListSortDirection lastDirection =
            ListSortDirection.Ascending;


        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        public MainWindow()
        {
            InitializeComponent();

            // TEST PROTECTED MAP SCANNER
            TestProtectedMapScanner(
                @"C:\Dota\Maps\Download\DotA_LoD_7.01f.w3x");
        }


        // =========================================================
        // COLUMN SORTING
        // =========================================================

        private void GridViewColumnHeader_Click(
            object sender,
            RoutedEventArgs e)
        {
            var headerClicked =
                e.OriginalSource as GridViewColumnHeader;

            if (headerClicked == null)
                return;

            string? header =
                headerClicked.Column?.Header?.ToString();

            if (string.IsNullOrEmpty(header))
                return;


            ListSortDirection direction;


            // Click same column again -> reverse direction
            if (headerClicked == lastHeaderClicked)
            {
                direction =
                    lastDirection ==
                    ListSortDirection.Ascending

                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;
            }
            else
            {
                direction =
                    ListSortDirection.Ascending;
            }


            lastHeaderClicked =
                headerClicked;

            lastDirection =
                direction;


            IEnumerable<TextureFile> sortedFiles;


            switch (header)
            {
                case "Filename":

                    sortedFiles =
                        direction ==
                        ListSortDirection.Ascending

                        ? filteredFiles
                            .OrderBy(f => f.Name)

                        : filteredFiles
                            .OrderByDescending(f => f.Name);

                    break;


                case "Extension":

                    sortedFiles =
                        direction ==
                        ListSortDirection.Ascending

                        ? filteredFiles
                            .OrderBy(f => f.Extension)

                        : filteredFiles
                            .OrderByDescending(f => f.Extension);

                    break;


                case "Size":

                    sortedFiles =
                        direction ==
                        ListSortDirection.Ascending

                        ? filteredFiles
                            .OrderBy(f => f.FileSize)

                        : filteredFiles
                            .OrderByDescending(f => f.FileSize);

                    break;


                default:
                    return;
            }


            filteredFiles =
                sortedFiles.ToList();


            FileList.ItemsSource =
                filteredFiles;
        }


        // =========================================================
        // SEARCH
        // =========================================================

        private void SearchBox_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            string search =
                SearchBox.Text.Trim();


            if (string.IsNullOrWhiteSpace(search))
            {
                filteredFiles =
                    new List<TextureFile>(allFiles);
            }
            else
            {
                filteredFiles =
                    allFiles
                        .Where(file =>
                            file.Name.Contains(
                                search,
                                StringComparison.OrdinalIgnoreCase))
                        .ToList();
            }


            FileList.ItemsSource =
                filteredFiles;


            FileCountText.Text =
                $"{filteredFiles.Count} Files";
        }


        // =========================================================
        // DRAG OVER
        // =========================================================

        private void Window_DragOver(
            object sender,
            System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(
                System.Windows.DataFormats.FileDrop))
            {
                e.Effects =
                    System.Windows.DragDropEffects.Copy;
            }
            else
            {
                e.Effects =
                    System.Windows.DragDropEffects.None;
            }


            e.Handled = true;
        }


        // =========================================================
        // DROP MDX FILE
        // =========================================================

        private void Window_Drop(
            object sender,
            System.Windows.DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(
                System.Windows.DataFormats.FileDrop))
            {
                return;
            }


            string[] files =
                (string[])e.Data.GetData(
                    System.Windows.DataFormats.FileDrop)!;


            foreach (string file in files)
            {
                if (!Path.GetExtension(file)
                    .Equals(
                        ".mdx",
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }


                try
                {
                    var model =
                        mdxParser.Read(file);


                    var mdxFile =
                        new MDXFile
                        {
                            Name =
                                Path.GetFileName(file),

                            FullPath =
                                file,

                            Version =
                                model.Version,

                            Textures =
                                new List<string>(
                                    model.Textures)
                        };


                    modelFiles.Add(
                        mdxFile);


                    ModelList.ItemsSource =
                        modelFiles;


                    System.Windows.MessageBox.Show(
                        $"Model loaded successfully!\n\n" +
                        $"Model: {mdxFile.Name}\n" +
                        $"Version: {mdxFile.Version}\n" +
                        $"Textures: {mdxFile.TextureCount}",
                        "MDX Model");
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(
                        ex.Message,
                        "MDX Error");
                }
            }
        }


        // =========================================================
        // OPEN FOLDER
        // =========================================================

        private void OpenFolder_Click(
            object sender,
            RoutedEventArgs e)
        {
            using FolderBrowserDialog dialog =
                new();


            if (dialog.ShowDialog() !=
                System.Windows.Forms.DialogResult.OK)
            {
                return;
            }


            string folderPath =
                dialog.SelectedPath;


            // Scan selected folder
            var files =
                scanner.ScanFolder(
                    folderPath);


            // Store all files
            allFiles =
                files;


            // Initially show everything
            filteredFiles =
                new List<TextureFile>(
                    allFiles);


            // Display files
            FileList.ItemsSource =
                filteredFiles;


            // Update UI
            FolderPathText.Text =
                folderPath;


            FolderFileCount.Text =
                $"{files.Count} texture files";


            FileCountText.Text =
                $"{files.Count} Files";


            StatusText.Text =
                "Folder loaded successfully.";
        }


        // =========================================================
        // FILE SELECTION
        // =========================================================

        private void FileList_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (FileList.SelectedItem
                is TextureFile texture)
            {
                PreviewPanel.Display(
                    texture);
            }
        }


        // =========================================================
        // MODEL SELECTION
        // =========================================================

        private void ModelList_SelectionChanged(
    object sender,
    SelectionChangedEventArgs e)
        {
            if (ModelList.SelectedItem
                is MDXFile model)
            {
                PreviewPanel.DisplayModel(model);

                MDXTextureList.ItemsSource =
                    model.Textures;

                MDXTextureCandidatesList.ItemsSource =
                    null;

                MDXTextureCandidateTitle.Text =
                    "Candidates";
            }
        }


        // =========================================================
        // PROTECTED MAP SCANNER
        // =========================================================

        private void TestProtectedMapScanner(
            string mapPath)
        {
            try
            {
                var protectedScanner =
                    new ProtectedMapScanner();


                // -------------------------------------------------
                // STEP 1:
                // Find all embedded BLP headers
                // -------------------------------------------------

                var blps =
                    protectedScanner.ReadBLPHeaders(
                        mapPath);


                // -------------------------------------------------
                // STEP 2:
                // Extract ALL BLPs
                // -------------------------------------------------

                string mapDirectory =
                    Path.GetDirectoryName(
                        mapPath)!;


                string outputFolder =
                    Path.Combine(
                        mapDirectory,
                        "ExtractedBLPs");


                int extracted =
                    protectedScanner.ExtractAllBLPs(
                        mapPath,
                        outputFolder);

                var index =
    protectedScanner.BuildExtractedBLPIndex(
        mapPath,
        outputFolder);

                extractedBLPs =
    index;

                ExtractedBLPList.ItemsSource =
                    extractedBLPs;


                // -------------------------------------------------
                // STEP 3:
                // Build information for the message
                // -------------------------------------------------

                string text =
                    $"Protected Map Scanner\n\n" +

                    $"BLP files found: " +
                    $"{blps.Count}\n\n" +

                    $"BLPs extracted: " +
                    $"{extracted}\n\n" +

                    $"Output folder:\n" +
                    $"{outputFolder}";


                // -------------------------------------------------
                // STEP 4:
                // Show first 10 BLPs
                // -------------------------------------------------

                text +=
                    $"\nIndexed BLPs: {index.Count}\n\n";


                int count =
                    Math.Min(
                        10,
                        blps.Count);

                int indexCount =
    Math.Min(10, index.Count);

                for (int i = 0;
                     i < indexCount;
                     i++)
                {
                    text +=
                        index[i].ToString() +
                        "\n";
                }


                for (int i = 0;
                     i < count;
                     i++)
                {
                    var blp =
                        blps[i];


                    text +=
                        $"#{blp.Index} | " +
                        $"{blp.Width}x{blp.Height} | " +
                        $"Compression: " +
                        $"{blp.Compression} | " +
                        $"Offset: " +
                        $"{blp.Offset}\n";
                }


                // -------------------------------------------------
                // STEP 5:
                // Show result
                // -------------------------------------------------

                System.Windows.MessageBox.Show(
                    text,
                    "Protected BLP Extraction");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    ex.ToString(),
                    "Scanner Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ExtractedBLPList_SelectionChanged(
    object sender,
    SelectionChangedEventArgs e)
        {
            if (ExtractedBLPList.SelectedItem
                is ExtractedBLPInfo blp)
            {
                var texture =
                    new TextureFile(
                        new FileInfo(blp.FullPath));

                PreviewPanel.Display(
                    texture);
            }
        }

        private void ExtractedBLPSearchBox_TextChanged(
    object sender,
    TextChangedEventArgs e)
        {
            string search =
                ExtractedBLPSearchBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(search))
            {
                ExtractedBLPList.ItemsSource =
                    extractedBLPs;

                return;
            }

            var results =
                extractedBLPs
                    .Where(blp =>
                        blp.FileName.Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase)

                        ||

                        $"{blp.Width}x{blp.Height}"
                            .Contains(
                                search,
                                StringComparison.OrdinalIgnoreCase)

                        ||

                        blp.Compression
                            .ToString()
                            .Contains(search,
                                StringComparison.OrdinalIgnoreCase))
                    .ToList();

            ExtractedBLPList.ItemsSource =
                results;
        }

        private void MDXTextureList_SelectionChanged(
    object sender,
    SelectionChangedEventArgs e)
        {
            if (MDXTextureList.SelectedItem is not string textureName)
                return;

            MDXTextureCandidateTitle.Text =
                $"Candidates for: {textureName}";

            // For now, show every extracted BLP.
            // We'll add actual matching logic next.
            MDXTextureCandidatesList.ItemsSource =
                extractedBLPs;
        }
    }
}