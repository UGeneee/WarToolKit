using System.Collections.Generic;
using System.Linq;
using WarToolKit.Models;

namespace WarToolKit.Services
{
    public class TextureResolver
    {
        private readonly List<TextureFile> _textures;

        public TextureResolver(List<TextureFile> textures)
        {
            _textures = textures;
        }

        public TextureFile? FindByName(string originalName)
        {
            string fileName =
                System.IO.Path.GetFileName(originalName);

            return _textures.FirstOrDefault(t =>
                t.Name.Equals(fileName,
                    System.StringComparison.OrdinalIgnoreCase));
        }
    }
}