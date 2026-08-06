using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarToolKit.Models
{
    public class TextureFile
    {
        public string Name { get; set; }

        public string FullPath { get; set; }

        public long FileSize { get; set; }

        public string Extension { get; set; }

        public TextureFile(FileInfo file)
        {
            Name = file.Name;
            FullPath = file.FullName;
            FileSize = file.Length;
            Extension = file.Extension;
        }
        public string FormattedFileSize
        {
            get
            {
                if (FileSize >= 1024 * 1024)
                    return $"{FileSize / 1024.0 / 1024.0:F2} MB";

                return $"{FileSize / 1024.0:F0} KB";
            }
        }

    }
}
