using System;
using System.Collections.Generic;
using System.IO;
using RawLauncher.Framework.Defreezer;
using RawLauncher.Framework.ExtensionClasses;

namespace RawLauncher.Framework.Utilities
{
    internal class UnitNameFile
    {

        public static readonly byte[] Offset = { 0xFF, 0xFE };

        public static readonly byte[] SearchPattern = { 0x0D, 0x00, 0x0A, 0x00 };

        public UnitNameFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException();
            if (Path.GetExtension(filePath) != ".txt")
                throw new NotSupportedException("Only txt files are supported");
            ByteArray = File.ReadAllBytes(filePath);
            FilePath = filePath;
        }

        public List<byte[]> Names
        {
            get
            {
                var pos = Offset.Length;
                var l = Offset.Length;

                var list = new List<byte[]>();

                while (pos != -1)
                {
                    pos = SearchAlgorithm.SearchKmp(ByteArray, SearchPattern, l);
                    if (pos == -1)
                        continue;

                    var nameBytes = new byte[Math.Abs(l -pos)];
                    var j = l;
                    for (var i = 0; i < nameBytes.Length -1; i++)
                        nameBytes[i] = ByteArray[j++];
                    list.Add(nameBytes);
                    l = pos + SearchPattern.Length;
                }

                return list;
            }
        }

        public string FilePath { get; }

        public byte[] ByteArray { get; set; }

        public void Shuffle()
        {
            var names = Names;
            names.Shuffle();


            var file = new List<byte>();

            file.AddRange(Offset);

            foreach (var bytes in names)
            {
                file.AddRange(bytes);
                file.AddRange(SearchPattern);
            }

            ByteArray = file.ToArray();
        }

        public void Save()
        {
            var writer = new BinaryWriter(File.OpenWrite(FilePath));
            writer.Write(ByteArray);
            writer.Close();
        }

    }
}
