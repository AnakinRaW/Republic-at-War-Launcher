using System;
using System.Diagnostics;
using System.IO;

namespace RawLauncherWPF.Updaters
{
    public abstract class AssemblyUpdater
    {
        private Version _latestVerion;
        public abstract string FileName { get; }
        
        public abstract string VersionsServerPath { get; }

        protected Version LatestVersion =>
            _latestVerion ?? (_latestVerion = VersionUtilities.GetLatestFileVersion(VersionsServerPath));

        protected Version CurrentVersion
        {
            get
            {
                if (!File.Exists(FileName))
                    throw new FileNotFoundException(FileName);
                return new Version(FileVersionInfo.GetVersionInfo(FileName).FileVersion);
            }
        }

        public void UpdateIfNewVersionExists()
        {
            if (LatestVersion == null)
                return;

            if (CurrentVersion < LatestVersion)
                Update();
        }

        protected abstract void Update();

        protected void DeleteCurrent()
        {
            if (File.Exists(FileName))
                File.Delete(FileName);
        }
    }
}