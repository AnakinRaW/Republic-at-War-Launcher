namespace RawLauncherWPF.Models
{
    public class RestoreFile
    {
        public string Name { get; set; }

        public string TargetPath { get; set; }

        public FileAction Action { get; set; }

        public string SourcePath { get; set; }

        public TargetType TargetType { get; set; }
    }

    public enum FileAction
    {
        Download,
        Delete
    }
}
