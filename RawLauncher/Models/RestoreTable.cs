using System;
using System.Collections.Generic;
using System.Linq;
using RawLauncher.Framework.Versioning;

namespace RawLauncher.Framework.Models
{
    public class RestoreTable
    {
        public RestoreTable(ModVersion version)
        {
            Files = new List<RestoreFile>();
            Version = version;
        }

        public ModVersion Version { get;}

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
