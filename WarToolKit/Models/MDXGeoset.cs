using System.Collections.Generic;
using System.Numerics;

namespace WarToolKit.Models
{
    public class MDXGeoset
    {
        public List<Vector3> Vertices { get; set; } = new();

        public List<Vector3> Normals { get; set; } = new();

        public List<Vector2> UVs { get; set; } = new();

        public List<int> Triangles { get; set; } = new();

        public int MaterialId { get; set; }

        public int VertexCount =>
            Vertices.Count;

        public int TriangleCount =>
            Triangles.Count / 3;
    }
}