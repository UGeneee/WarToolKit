namespace WarToolKit.Models
{
    public class MDXChunk
    {
        public string Name { get; set; } = "";

        public int Size { get; set; }

        public long Offset { get; set; }

        public override string ToString()
        {
            return $"{Name} ({Size} bytes)";
        }
    }
}