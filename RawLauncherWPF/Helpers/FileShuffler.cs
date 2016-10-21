using System;
using System.IO;

namespace RawLauncherWPF.Helpers
{
    public static class FileShuffler
    {
        public static void ShuffleFiles(string directory)
        {
            if (!Directory.Exists(directory))
                throw new DirectoryNotFoundException(nameof(directory));
            foreach (var file in Directory.EnumerateFiles(directory, "*.txt", SearchOption.TopDirectoryOnly))
            {
                var unitFile = new UnitNameFile(file);
                unitFile.Shuffle();
                unitFile.Save();
            }
        }
    }
}
