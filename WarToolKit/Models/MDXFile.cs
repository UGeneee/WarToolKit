using System.Collections.Generic;

namespace WarToolKit.Models
{
    public class MDXFile
    {
        public string Name { get; set; } = "";

        public string FullPath { get; set; } = "";

        public int Version { get; set; }

        public List<string> Textures { get; set; } = new();

        public List<MDXGeoset> Geosets { get; set; } = new();

        public int TextureCount =>
            Textures.Count;

        public int GeosetCount =>
            Geosets.Count;

        public int VertexCount
        {
            get
            {
                int count = 0;

                foreach (var geoset in Geosets)
                    count += geoset.Vertices.Count;

                return count;
            }
        }

        public int TriangleCount
        {
            get
            {
                int count = 0;

                foreach (var geoset in Geosets)
                    count += geoset.TriangleCount;

                return count;
            }
        }
    }
}