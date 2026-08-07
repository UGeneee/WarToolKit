using System.IO;
using WarToolKit.Models;
using WarToolKit.Utilities;
using System.Text;
using System.Text.RegularExpressions;

namespace WarToolKit.Parsers
{
    public class MDXParser
    {
        public MDXModel Read(string path)
        {
            using BinaryReader reader = new(File.OpenRead(path));

            MDXModel model = new();

            model.Name = Path.GetFileName(path);

            string magic = reader.ReadFourCC();

            if (magic != "MDLX")
                throw new InvalidDataException("Not an MDX model.");

            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                if (reader.BaseStream.Length - reader.BaseStream.Position < 8)
                    break;

                string chunkName = reader.ReadFourCC();

                int chunkSize = reader.ReadInt32();

                model.Chunks.Add(new MDXChunk
                {
                    Name = chunkName,
                    Size = chunkSize,
                    Offset = reader.BaseStream.Position - 8
                });

                if (chunkName == "VERS")
                {
                    model.Version = reader.ReadInt32();

                    reader.BaseStream.Seek(chunkSize - 4, SeekOrigin.Current);
                }
                else if (chunkName == "TEXS")
                {
                    byte[] texData = reader.ReadBytes(chunkSize);

                    string text = Encoding.ASCII.GetString(texData);

                    var matches = Regex.Matches(
                        text,
                        @"[A-Za-z0-9_\\./-]+\.blp",
                        RegexOptions.IgnoreCase);

                    foreach (Match match in matches)
                    {
                        if (!model.Textures.Contains(match.Value))
                        {
                            model.Textures.Add(match.Value);
                        }
                    }
                }
                else
                {
                    reader.BaseStream.Seek(chunkSize, SeekOrigin.Current);
                }

            }

            return model;
        }
    }
}