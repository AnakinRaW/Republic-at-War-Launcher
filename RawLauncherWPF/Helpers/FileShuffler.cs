using System;
using System.IO;
using System.Linq;

namespace RawLauncherWPF.Helpers
{
    public class FileShuffler
    {
        private string FilePath { get; }
        public FileShuffler(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException(nameof(filePath));
            if (Path.GetExtension(filePath) != ".txt")
                throw new NotSupportedException("Only txt files are supported");
            FilePath = filePath;
        }

        public void Shuffle()
        {
            var lines = File.ReadAllLines(FilePath);
            var rnd = new Random();
            lines = lines.OrderBy(line => rnd.Next()).ToArray();
            File.WriteAllLines(FilePath, lines);
        }

        public static void ShuffleFiles(string directory)
        {
            if (!Directory.Exists(directory))
                throw new DirectoryNotFoundException(nameof(directory));
            foreach (var file in Directory.EnumerateFiles(directory, "*.txt", SearchOption.TopDirectoryOnly))
                new FileShuffler(file).Shuffle();
        }
    }
}
