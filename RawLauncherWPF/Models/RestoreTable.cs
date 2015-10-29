using System;
using System.Collections.Generic;
using System.Linq;

namespace RawLauncherWPF.Models
{
    public class RestoreTable
    {
        public RestoreTable()
        {
            Files = new List<RestoreFile>();
        }
        public Version Version { get; set; }

        public List<RestoreFile> Files;

        public List<RestoreFile> GetFilesOfType(TargetType type)
        {
            return Files.Where(files => files.TargetType == type).ToList();
        } 
    }
}
