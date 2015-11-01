using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RawLauncherWPF.Defreezer
{
    internal class Defreezer
    {
        private static readonly byte[] ReplacePattern =
        {
            0X01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x80, 0x00, 0x00,
            0x00, 0x00, 0x18, 0x00, 0x00, 0x00, 0x00, 0x04
        };

        public Defreezer(SaveGame saveGame)
        {
            if (saveGame == null)
                throw new NoNullAllowedException();
            SaveGame = saveGame;
        }

        public SaveGame SaveGame { get; }

        private IEnumerable<UnitRef> URefs { get; set; }

        public void DefreezeSaveGame()
        {
            URefs = SaveGame.GetAllUnitRefs().OrderBy(r => r.IntId);
            var i = 0;
            foreach (var uRef in URefs)
            {
                var byteArray = BitConverter.GetBytes(i);
                uRef.FirstId = byteArray;
                uRef.IntId = i;
                SaveGame.ByteArray[uRef.Position] = uRef.FirstId[0];
                SaveGame.ByteArray[uRef.Position + 1] = uRef.FirstId[1];
                SaveGame.ByteArray[uRef.Position + 2] = uRef.FirstId[2];
                i++;
            }
            var tasks = new List<Task>();
            var pos = 0;
            while (pos != -1)
            {
                pos = SearchAlgorithm.SearchKmp(SaveGame.ByteArray, ReplacePattern, pos);
                if (pos == -1)
                    continue;
                pos = pos + 18;
                byte[] tmp =
                {
                    SaveGame.ByteArray[pos], SaveGame.ByteArray[pos + 1], SaveGame.ByteArray[pos + 2],
                    SaveGame.ByteArray[pos + 3], SaveGame.ByteArray[pos + 4], SaveGame.ByteArray[pos + 5],
                    SaveGame.ByteArray[pos + 6], SaveGame.ByteArray[pos + 7], SaveGame.ByteArray[pos + 8],
                    SaveGame.ByteArray[pos + 9]
                };
                var bpos = pos;
                tasks.Add(Task.Factory.StartNew(() => RepaceBytes(tmp, bpos)));
            }
            Task.WaitAll(tasks.ToArray());
            var writer = new BinaryWriter(File.OpenWrite(SaveGame.FilePath));
            writer.Write(SaveGame.ByteArray);
            writer.Close();
        }

        private void RepaceBytes(byte[] tmp, int bpos)
        {
            foreach (var uRef in URefs.Where(uRef => uRef.CompleteId.SequenceEqual(tmp)))
            {
                SaveGame.ByteArray[bpos] = uRef.FirstId[0];
                SaveGame.ByteArray[bpos + 1] = uRef.FirstId[1];
                SaveGame.ByteArray[bpos + 2] = uRef.FirstId[2];
            }
        }
    }
}