using System.IO;

namespace WarToolKit.Services
{
    public class BLPReader
    {
        public IBLPReader GetReader(string file)
        {
            using var reader =
                new BinaryReader(File.OpenRead(file));

            string magic =
                new string(reader.ReadChars(4));

            return magic switch
            {
                "BLP1" => new BLP1Reader(),
                "BLP2" => new BLP2Reader(),

                _ => throw new InvalidDataException(
                    $"Unknown BLP format: {magic}")
            };
        }

        public BLPHeader ReadHeader(string file)
        {
            return GetReader(file).ReadHeader(file);
        }

        public byte[] ReadFirstMip(string file)
        {
            return GetReader(file).ReadFirstMip(file);
        }
    }
}