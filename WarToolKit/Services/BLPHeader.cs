namespace WarToolKit.Services
{
    public class BLPHeader
    {

        public string Magic { get; set; } = "";

        public int Version { get; set; }

        public BLPCompression Compression { get; set; }

        public byte AlphaBits { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int Extra { get; set; }

        public bool HasMipmaps { get; set; }

        public int[] MipOffsets { get; } = new int[16];

        public int[] MipSizes { get; } = new int[16];
    }
}