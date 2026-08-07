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

            // Click the same column again -> reverse direction
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
                if (Path.GetExtension(file)
                    .Equals(
                        ".mdx",
                        StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        var model =
                            mdxParser.Read(file);

                        string text =
                            $"Model: {model.Name}\n" +
                            $"Version: {model.Version}\n\n";

                        foreach (string tex in model.Textures)
                        {
                            text += tex + "\n";
                        }

                        System.Windows.MessageBox.Show(
                            text,
                            "MDX Information");
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(
                            ex.Message,
                            "MDX Error");
                    }
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
            using FolderBrowserDialog dialog = new();


            if (dialog.ShowDialog() !=
                System.Windows.Forms.DialogResult.OK)
            {
                return;
            }


            // THIS was the missing variable
            string folderPath =
                dialog.SelectedPath;


            // Scan the selected folder
            var files =
                scanner.ScanFolder(folderPath);


            // Store all files
            allFiles =
                files;


            // Initially show everything
            filteredFiles =
                new List<TextureFile>(allFiles);


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
                PreviewPanel.Display(texture);
            }
        }
    }
}