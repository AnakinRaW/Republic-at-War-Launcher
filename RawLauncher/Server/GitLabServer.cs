using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using RawLauncher.Framework.Configuration;
using RawLauncher.Framework.ExtensionClasses;
using RawLauncher.Framework.Utilities;
using RawLauncher.Framework.Versioning;

namespace RawLauncher.Framework.Server
{
    [Export(typeof(IHostServer))]
    [Export(typeof(IVersionServer))]
    public class GitLabServer : GitServerBase
    {
        protected string ModVersionListRelativePath = $"master/{Config.AvailableModVersionsFileName}";

        public const string ServerUrl = "https://gitlab.com/Republic-at-War/Republic-At-War/raw/";

        public override string ServerRootAddress { get; }

        public GitLabServer()
        {
            ServerRootAddress = ServerUrl;
        }

        public override string GetRescueFilePath(RescueFileType type, ModVersion version)
        {
            string fileName;
            switch (type)
            {
                case RescueFileType.Check:
                    fileName = Config.CheckFileFileName;
                    break;
                case RescueFileType.UpdateRestore:
                    fileName = Config.RestoreFileFileName;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            return version + @"\RescueFiles\" + fileName;
        }

        public override IEnumerable<ModVersion> GetAllVersions()
        {
            var data = DownloadString(ModVersionListRelativePath).ToStream();
            return VersionUtilities.SerializeVersionsToList(data);
        }
    }
}