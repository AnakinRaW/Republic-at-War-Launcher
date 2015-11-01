using System.Collections.Generic;
using System.IO;

namespace RawLauncherWPF.Defreezer
{
    public abstract class SaveGame
    {
        protected SaveGame(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException(nameof(filePath));
            FilePath = filePath;
            ByteArray = File.ReadAllBytes(filePath);
        }

        public string FilePath { get; }

        public abstract string Name { get; }

        public byte[] ByteArray { get; set; }

        public List<UnitRef> GetAllUnitRefs()
        {
            var uRefs = new List<UnitRef>();
            var l = 0;
            var pos = 0;

            while (pos != -1)
            {
                pos = SearchAlgorithm.SearchKmp(ByteArray, UnitRef.SearchPattern, l);
                if (pos == -1)
                    continue;
                l = pos + 17;
                uRefs.Add(new UnitRef(l, new[] {ByteArray[l], ByteArray[l + 1], ByteArray[l + 2]},
                    new[] {ByteArray[l + 6], ByteArray[l + 7], ByteArray[l + 8], ByteArray[l + 9]}));
            }
            return uRefs;
        }
    }
}