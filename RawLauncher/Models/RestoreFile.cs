using System.IO;

namespace RawLauncher.Framework.Models
{
    public class RestoreFile
    {
        public string Name { get; set; }

        public string TargetPath { get; set; }

        public FileAction Action { get; set; }

        public string SourcePath { get; set; }

        public TargetType TargetType { get; set; }

        public static RestoreFile CreateResotreFile(FileContainerFile file, FileAction action)
        {
            var restoreFile = new RestoreFile
            {
                Name = file.Name,
                TargetPath = file.TargetPath,
                SourcePath = file.SourcePath,
                TargetType = file.TargetType,
                Action = action
            };
            return restoreFile;
        }

        public static RestoreFile CreateDeleteFile(string file, TargetType type)
        {
            return new RestoreFile
            {
                Name = Path.GetFileName(file),
                TargetType = type,
                TargetPath = Path.GetFullPath(file),
                Action = FileAction.Delete
            };
        }
    }
    
    public enum FileAction
    {
        Download,
        Delete
    }
}
