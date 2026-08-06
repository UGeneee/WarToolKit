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
    }
}
