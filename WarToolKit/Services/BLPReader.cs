using System.IO;
using System.Text;
using WarToolKit.Models;

namespace WarToolKit.Services
{
    public class BLPReader
    {
        public BLPInfo Read(string path)
        {
            using BinaryReader reader = new(File.OpenRead(path));

            string magic = Encoding.ASCII.GetString(reader.ReadBytes(4));

            BLPInfo info = new()
            {
                Version = magic
            };

            if (magic == "BLP1")
            {
                int compression = reader.ReadInt32();
                int flags = reader.ReadInt32();

                info.Width = reader.ReadInt32();
                info.Height = reader.ReadInt32();

                info.Compression = compression == 0
                    ? "JPEG"
                    : "Palette";
            }
            else if (magic == "BLP2")
            {
                byte compression = reader.ReadByte();
                byte alphaDepth = reader.ReadByte();
                byte alphaType = reader.ReadByte();
                byte hasMipmaps = reader.ReadByte();

                info.Width = reader.ReadInt32();
                info.Height = reader.ReadInt32();

                info.Compression = compression switch
                {
                    1 => "DXT",
                    2 => "Uncompressed",
                    _ => "Unknown"
                };

                info.Mipmaps = hasMipmaps;
            }

            return info;
        }
    }
}