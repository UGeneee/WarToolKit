using System.IO;

namespace WarToolKit.Services
{
    public class BLP2Reader : IBLPReader
    {
        public BLPHeader ReadHeader(string file)
        {
            using var reader =
                new BinaryReader(File.OpenRead(file));

            var header = new BLPHeader();

            header.Magic =
                new string(reader.ReadChars(4));

            if (header.Magic != "BLP2")
                throw new InvalidDataException(
                    "Not a BLP2 file.");

            header.Version = 2;

            header.Compression =
                (BLPCompression)reader.ReadByte();

            header.AlphaBits =
                reader.ReadByte();

            // Alpha encoding
            reader.ReadByte();

            // Has mipmaps
            header.HasMipmaps =
                reader.ReadByte() != 0;

            header.Width =
                reader.ReadInt32();

            header.Height =
                reader.ReadInt32();

            for (int i = 0; i < 16; i++)
            {
                header.MipOffsets[i] =
                    reader.ReadInt32();
            }

            for (int i = 0; i < 16; i++)
            {
                header.MipSizes[i] =
                    reader.ReadInt32();
            }

            return header;
        }

        public byte[] ReadFirstMip(string file)
        {
            var header = ReadHeader(file);

            if (header.MipOffsets[0] <= 0 ||
                header.MipSizes[0] <= 0)
            {
                throw new InvalidDataException(
                    "BLP2 file does not contain a valid first mipmap.");
            }

            using var reader =
                new BinaryReader(File.OpenRead(file));

            reader.BaseStream.Position =
                header.MipOffsets[0];

            return reader.ReadBytes(
                header.MipSizes[0]);
        }
    }
}