using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarToolKit.Models;

namespace WarToolKit.Services
{
    class FileScanner
    {
        public List<TextureFile> ScanFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                return new List<TextureFile>();

            return Directory
                .GetFiles(folderPath, "*.blp", SearchOption.AllDirectories)
                .Select(file => new TextureFile(new FileInfo(file)))
                .OrderBy(file => file.Name)
                .ToList();
        }
    }
}
