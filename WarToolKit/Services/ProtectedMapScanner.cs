using System;
using System.Collections.Generic;
using System.IO;
using WarToolKit.Models;

namespace WarToolKit.Services
{
    public class ProtectedMapScanner
    {
        public List<long> FindBLPFiles(string mapPath)
        {
            var results = new List<long>();

            using var stream = File.OpenRead(mapPath);

            long length = stream.Length;

            byte[] buffer = new byte[4];

            while (stream.Position <= length - 4)
            {
                long position = stream.Position;

                int bytesRead =
                    stream.Read(buffer, 0, 4);

                if (bytesRead < 4)
                    break;

                if (buffer[0] == 0x42 &&
                    buffer[1] == 0x4C &&
                    buffer[2] == 0x50 &&
                    buffer[3] == 0x31)
                {
                    results.Add(position);
                }

                stream.Position =
                    position + 1;
            }

            return results;
        }


        public List<ProtectedBLPInfo> ReadBLPHeaders(
            string mapPath)
        {
            var results =
                new List<ProtectedBLPInfo>();

            var offsets =
                FindBLPFiles(mapPath);

            using var stream =
                File.OpenRead(mapPath);

            using var reader =
                new BinaryReader(stream);

            for (int i = 0;
                 i < offsets.Count;
                 i++)
            {
                long offset =
                    offsets[i];

                try
                {
                    stream.Position =
                        offset;

                    string magic =
                        new string(
                            reader.ReadChars(4));

                    if (magic != "BLP1")
                        continue;

                    int compression =
                        reader.ReadInt32();

                    int alphaBits =
                        reader.ReadInt32();

                    int width =
                        reader.ReadInt32();

                    int height =
                        reader.ReadInt32();

                    // Skip mip offsets except mip 0
                    int firstMipOffset =
                        reader.ReadInt32();

                    // Skip remaining mip offsets
                    for (int mip = 1;
                         mip < 16;
                         mip++)
                    {
                        reader.ReadInt32();
                    }

                    // Read mip 0 size
                    int firstMipSize =
                        reader.ReadInt32();

                    results.Add(
                        new ProtectedBLPInfo
                        {
                            Index = i,
                            Offset = offset,
                            Width = width,
                            Height = height,
                            Compression = compression,
                            AlphaBits = alphaBits,
                            FirstMipOffset =
                                firstMipOffset,
                            FirstMipSize =
                                firstMipSize
                        });
                }
                catch
                {
                    // Ignore invalid/corrupted entries.
                }
            }

            return results;
        }

        public byte[] ExtractBLP(
    string mapPath,
    ProtectedBLPInfo blp)
        {
            using var stream =
                File.OpenRead(mapPath);

            using var reader =
                new BinaryReader(stream);

            stream.Position =
                blp.Offset;

            // Read the BLP header.
            reader.ReadBytes(4); // BLP1

            reader.ReadInt32(); // Compression
            reader.ReadInt32(); // AlphaBits
            reader.ReadInt32(); // Width
            reader.ReadInt32(); // Height

            // Read all mip offsets.
            int[] mipOffsets =
                new int[16];

            for (int i = 0; i < 16; i++)
            {
                mipOffsets[i] =
                    reader.ReadInt32();
            }

            // Read all mip sizes.
            int[] mipSizes =
                new int[16];

            for (int i = 0; i < 16; i++)
            {
                mipSizes[i] =
                    reader.ReadInt32();
            }

            // Find the last valid mipmap.
            int endOffset = 0;

            for (int i = 0; i < 16; i++)
            {
                if (mipOffsets[i] > 0 &&
                    mipSizes[i] > 0)
                {
                    int end =
                        mipOffsets[i] +
                        mipSizes[i];

                    if (end > endOffset)
                        endOffset = end;
                }
            }

            if (endOffset <= 0)
                throw new InvalidDataException(
                    "Could not determine BLP data size.");

            long absoluteEnd =
                blp.Offset + endOffset;

            long size =
                absoluteEnd - blp.Offset;

            if (size <= 0 ||
                size > int.MaxValue)
            {
                throw new InvalidDataException(
                    "Invalid BLP size.");
            }

            stream.Position =
                blp.Offset;

            return reader.ReadBytes((int)size);
        }
        public int ExtractAllBLPs(
    string mapPath,
    string outputFolder)
        {
            Directory.CreateDirectory(outputFolder);

            var blps =
                ReadBLPHeaders(mapPath);

            int extracted = 0;

            foreach (var blp in blps)
            {
                try
                {
                    byte[] data =
                        ExtractBLP(
                            mapPath,
                            blp);

                    string fileName =
                        $"BLP_{blp.Index:D5}.blp";

                    string outputPath =
                        Path.Combine(
                            outputFolder,
                            fileName);

                    File.WriteAllBytes(
                        outputPath,
                        data);

                    extracted++;
                }
                catch
                {
                    // Ignore individual invalid BLPs
                }
            }

            return extracted;
        }

        public List<ExtractedBLPInfo> BuildExtractedBLPIndex(
    string mapPath,
    string outputFolder)
        {
            var results =
                new List<ExtractedBLPInfo>();

            var blps =
                ReadBLPHeaders(mapPath);

            foreach (var blp in blps)
            {
                string fileName =
                    $"BLP_{blp.Index:D5}.blp";

                string fullPath =
                    Path.Combine(
                        outputFolder,
                        fileName);

                if (!File.Exists(fullPath))
                    continue;

                var fileInfo =
                    new FileInfo(fullPath);

                results.Add(
                    new ExtractedBLPInfo
                    {
                        Index = blp.Index,

                        FileName =
                            fileName,

                        FullPath =
                            fullPath,

                        Width =
                            blp.Width,

                        Height =
                            blp.Height,

                        Compression =
                            blp.Compression,

                        AlphaBits =
                            blp.AlphaBits,

                        FileSize =
                            fileInfo.Length
                    });
            }

            return results;
        }

    }

}