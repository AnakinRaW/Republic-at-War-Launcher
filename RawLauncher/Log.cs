using System;
using System.IO;

namespace RawLauncher.Framework
{
    public class Log
    {
        private static readonly string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "RaWLog.txt");

        public static void Write(string message)
        {
#if DEBUG
            using (StreamWriter streamWriter = new StreamWriter(FilePath, true))
            {
                var dateMessage = $"{DateTime.Now}\t{message}";
                streamWriter.WriteLine(dateMessage);
                streamWriter.Close();
            }
#endif
        }
    }
}
