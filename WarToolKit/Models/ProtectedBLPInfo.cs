using System;

namespace WarToolKit.Models
{
    public class ProtectedBLPInfo
    {
        public int Index { get; set; }

        public long Offset { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int Compression { get; set; }

        public int AlphaBits { get; set; }

        public int FirstMipOffset { get; set; }

        public int FirstMipSize { get; set; }

        public override string ToString()
        {
            return
                $"BLP #{Index}\n" +
                $"Offset: {Offset}\n" +
                $"Resolution: {Width} x {Height}\n" +
                $"Compression: {Compression}\n" +
                $"Alpha Bits: {AlphaBits}\n" +
                $"First Mip Offset: {FirstMipOffset}\n" +
                $"First Mip Size: {FirstMipSize}";
        }
    }
}