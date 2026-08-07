using System;
using System.IO;

namespace WarToolKit.Services
{
    public class BLP1Reader : IBLPReader
    {
        public BLPHeader ReadHeader(string file)
        {
            using var reader =
                new BinaryReader(File.OpenRead(file));

            var header = new BLPHeader();

            // Magic: BLP1
            header.Magic =
                new string(reader.ReadChars(4));

            if (header.Magic != "BLP1")
            {
                throw new InvalidDataException(
                    "Not a BLP1 file.");
            }

            header.Version = 1;

            // Compression
            header.Compression =
                (BLPCompression)reader.ReadInt32();

            // Alpha bits
            header.AlphaBits =
                (byte)reader.ReadInt32();

            // Width
            header.Width =
                reader.ReadInt32();

            // Height
            header.Height =
                reader.ReadInt32();

            // Extra / picture type
            header.Extra =
                reader.ReadInt32();

            // Mipmap flag
            header.HasMipmaps =
                reader.ReadInt32() != 0;

            // Mipmap offsets
            for (int i = 0; i < 16; i++)
            {
                header.MipOffsets[i] =
                    reader.ReadInt32();
            }

            // Mipmap sizes
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

            if (header.Compression != BLPCompression.JPEG)
            {
                throw new InvalidDataException(
                    "This BLP1 texture is not JPEG compressed.");
            }

            if (header.MipOffsets[0] <= 0 ||
                header.MipSizes[0] <= 0)
            {
                throw new InvalidDataException(
                    "BLP1 file does not contain a valid first mipmap.");
            }

            using var reader =
                new BinaryReader(File.OpenRead(file));

            // BLP1 header ends at byte 156
            reader.BaseStream.Position = 156;

            // JPEG shared header size
            int jpegHeaderSize =
                reader.ReadInt32();

            if (jpegHeaderSize <= 0)
            {
                throw new InvalidDataException(
                    "Invalid JPEG header size.");
            }

            // Read the shared JPEG header
            byte[] jpegHeader =
                reader.ReadBytes(jpegHeaderSize);

            if (jpegHeader.Length != jpegHeaderSize)
            {
                throw new InvalidDataException(
                    "Could not read JPEG header.");
            }

            // Go to mipmap #0
            reader.BaseStream.Position =
                header.MipOffsets[0];

            // Read mipmap JPEG data
            byte[] mipData =
                reader.ReadBytes(header.MipSizes[0]);

            if (mipData.Length != header.MipSizes[0])
            {
                throw new InvalidDataException(
                    "Could not read first mipmap.");
            }

            // Combine JPEG header + mipmap
            byte[] jpegData =
                new byte[jpegHeader.Length + mipData.Length];

            Buffer.BlockCopy(
                jpegHeader,
                0,
                jpegData,
                0,
                jpegHeader.Length);

            Buffer.BlockCopy(
                mipData,
                0,
                jpegData,
                jpegHeader.Length,
                mipData.Length);

            return jpegData;
        }

        public byte[] ReadPaletteMip(string file)
        {
            var header = ReadHeader(file);

            if (header.Compression != BLPCompression.Palette)
            {
                throw new InvalidDataException(
                    "This BLP1 texture is not palette compressed.");
            }

            if (header.MipOffsets[0] <= 0 ||
                header.MipSizes[0] <= 0)
            {
                throw new InvalidDataException(
                    "BLP1 file does not contain a valid first mipmap.");
            }

            using var reader =
                new BinaryReader(File.OpenRead(file));

            // -------------------------------------------------
            // BLP1 palette
            // -------------------------------------------------
            //
            // BLP1 header = 156 bytes
            // Palette = 256 entries × 4 bytes
            //
            // Each palette entry is stored as:
            //
            // Blue
            // Green
            // Red
            // Alpha
            //
            // -------------------------------------------------

            reader.BaseStream.Position = 156;

            byte[] palette =
                reader.ReadBytes(256 * 4);

            if (palette.Length != 256 * 4)
            {
                throw new InvalidDataException(
                    "Could not read the BLP1 palette.");
            }

            // -------------------------------------------------
            // Read first mipmap
            // -------------------------------------------------

            reader.BaseStream.Position =
                header.MipOffsets[0];

            byte[] indices =
                reader.ReadBytes(header.MipSizes[0]);

            if (indices.Length != header.MipSizes[0])
            {
                throw new InvalidDataException(
                    "Could not read the first palette mipmap.");
            }

            // -------------------------------------------------
            // Convert palette indices to RGBA
            // -------------------------------------------------

            int pixelCount =
                header.Width * header.Height;

            byte[] rgba =
                new byte[pixelCount * 4];

            for (int i = 0; i < pixelCount; i++)
            {
                if (i >= indices.Length)
                    break;

                int paletteIndex =
                    indices[i];

                int palettePosition =
                    paletteIndex * 4;

                byte blue =
                    palette[palettePosition];

                byte green =
                    palette[palettePosition + 1];

                byte red =
                    palette[palettePosition + 2];

                byte alpha =
                    palette[palettePosition + 3];

                int output =
                    i * 4;

                rgba[output] = blue;
                rgba[output + 1] = green;
                rgba[output + 2] = red;
                rgba[output + 3] = alpha;
            }

            return rgba;
        }
    }
}