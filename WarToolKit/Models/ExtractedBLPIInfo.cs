using System;

namespace WarToolKit.Models
{
    public class ExtractedBLPInfo
    {
        public int Index { get; set; }

        public string FileName { get; set; } = "";

        public string FullPath { get; set; } = "";

        public int Width { get; set; }

        public int Height { get; set; }

        public int Compression { get; set; }

        public int AlphaBits { get; set; }

        public long FileSize { get; set; }

        public override string ToString()
        {
            return
                $"{FileName} | " +
                $"{Width}x{Height} | " +
                $"Compression: {Compression} | " +
                $"{FileSize:N0} bytes";
        }
    }
}