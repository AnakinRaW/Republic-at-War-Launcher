using System;
using System.IO;
using System.Security.AccessControl;

namespace RawLauncherWPF.ExtensionClasses
{
    public static class StreamExtension
    {
        /// <summary>
        /// Reads the stream and converts is to ASCII chars. Writes the result into a file or overrites the existing
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="path"></param>
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
            File.WriteAllText(path, fileContent);
        }

        /// <summary>
        /// Checks if the Stream is empty
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>Returns true if Empty</returns>
        public static bool IsEmpty(this Stream stream)
        {
            return stream.Length == 0 || stream == Stream.Null;
        }
    }
}
