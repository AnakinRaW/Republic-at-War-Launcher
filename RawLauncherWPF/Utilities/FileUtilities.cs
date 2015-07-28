using System.IO;
using RawLauncherWPF.ExtensionClasses;

namespace RawLauncherWPF.Utilities
{
    public static class FileUtilities
    {
        public static Stream FileToStream(string file)
        {
            if (!File.Exists(file))
                throw new FileNotFoundException();
            return new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
        }
    }
}
