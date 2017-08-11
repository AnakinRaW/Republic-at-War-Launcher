using System;
using System.IO;

namespace RawLauncher.Framework.Utilities
{
    public static class RestoreUpdateUtilities
    {
        public static bool IgnoreFile(string file, RestoreUpdateOperation operation)
        {
            if (file == null)
                throw new NullReferenceException(nameof(file));
            if (operation == RestoreUpdateOperation.Restore)
            {
                if (Path.GetFullPath(file).Contains("\\Text\\"))
                    return true;
            }
            if (Path.GetFullPath(file).Contains("\\Audio\\Speech\\"))
                return true;
            var s = new FileInfo(file).Directory?.Name;
            if (s == "Audio")
                return true;
            if (Path.GetFileName(file).Contains("Speech.meg"))
                return true;
            return false;
        }
    }

    public enum RestoreUpdateOperation
    {
        Update,
        Restore
    }
}
