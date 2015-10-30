using System;
using System.Collections.Generic;
using System.Linq;

namespace RawLauncherWPF.Models
{
    public class RestoreTable
    {
        public RestoreTable(Version version)
        {
            Files = new List<RestoreFile>();
            Version = version;
        }

        public Version Version { get;}

        public List<RestoreFile> Files;

        public List<RestoreFile> GetFilesOfType(TargetType type)
        {
            return Files.Where(files => files.TargetType == type).ToList();
        }

        public List<RestoreFile> GetFilesOfAction(FileAction action)
        {
            return Files.Where(files => files.Action == action).ToList();
        } 
    }
}
