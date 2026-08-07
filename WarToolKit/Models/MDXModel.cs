using System.Collections.Generic;

namespace WarToolKit.Models
{
    public class MDXModel
    {
        public string Name { get; set; } = "";

        public int Version { get; set; }

        public List<MDXChunk> Chunks { get; } = new();

        public List<string> Textures { get; } = new();
    }
}