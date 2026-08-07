namespace WarToolKit.Services
{
    public interface IBLPReader
    {
        BLPHeader ReadHeader(string file);

        byte[] ReadFirstMip(string file);
    }
}