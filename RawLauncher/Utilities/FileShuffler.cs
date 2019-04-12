using System.IO;

namespace RawLauncher.Framework.Utilities
{
    public static class FileShuffler
    {
        public static void ShuffleFiles(string directory)
        {
            if (!Directory.Exists(directory))
                return;
            try
            {
                foreach (var file in Directory.EnumerateFiles(directory, "*.txt", SearchOption.TopDirectoryOnly))
                {
                    var unitFile = new UnitNameFile(file);
                    unitFile.Shuffle();
                    unitFile.Save();
                }
            }
            catch
            {
            }

        }
    }
}
