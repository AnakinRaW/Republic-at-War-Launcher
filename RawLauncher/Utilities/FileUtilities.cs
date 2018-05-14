using System;
using System.IO;

namespace RawLauncher.Framework.Utilities
{
    public static class FileUtilities
    {
        public static Stream FileToStream(string file)
        {
            if (!File.Exists(file))
                throw new FileNotFoundException();
            return new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            var diSource = new DirectoryInfo(sourceDirectory);
            var diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (var fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (var diSourceSubDir in source.GetDirectories())
            {
                var nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        public static void DeleteDirectory(string path)
        {
            foreach (var directory in Directory.GetDirectories(path))
            {
                DeleteDirectory(directory);
            }
            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException)
            {
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException)
            {
                Directory.Delete(path, true);
            }
        }

        public static bool IsFileReadOnly(string path)
        {
            return File.Exists(path) && File.GetAttributes(path).HasFlag(FileAttributes.ReadOnly);
        }

        public static void MakeFileReadOnly(string path)
        {
            if (!File.Exists(path))
                return;
            var attributes = File.GetAttributes(path);
            File.SetAttributes(path, attributes | FileAttributes.ReadOnly);
        }

        public static void MakeFileWriteable(string path)
        {
            if (!File.Exists(path))
                return;
            var attributes = File.GetAttributes(path);
            attributes = attributes & ~FileAttributes.ReadOnly;
            File.SetAttributes(path, attributes);
        }
    }
}
