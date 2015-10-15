using System;
using System.IO;
using System.Security.AccessControl;

namespace RawLauncherWPF.ExtensionClasses
{
    public static class StreamExtension
    {
        public static void ToFile(this Stream stream, string path)
        {
            if (stream.IsEmpty() || path == null)
                throw new ArgumentNullException();
            stream.Position = 0;
            var reader = new StreamReader(stream);
            var fileContent = reader.ReadToEnd();

            var dirPath = Path.GetDirectoryName(path);
            if (dirPath == null)
                return;
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            if (!File.Exists(path))
            {
                File.WriteAllText(path, fileContent);
            }
        }

        public static bool IsEmpty(this Stream stream)
        {
            return stream.Length == 0 || stream == Stream.Null;
        }
    }
}
