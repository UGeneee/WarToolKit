using System.IO;
using System.Text;

namespace WarToolKit.Utilities
{
    public static class BinaryReaderExtensions
    {
        public static string ReadFourCC(this BinaryReader reader)
        {
            return Encoding.ASCII.GetString(reader.ReadBytes(4));
        }
    }
}