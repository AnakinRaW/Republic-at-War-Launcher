using System;
using System.Diagnostics;
using System.IO;
using RawLauncherWPF.Annotations;

namespace RawLauncherWPF.Updaters
{
    public abstract class AssemblyUpdater
    {
        private Version _latestVersion;
        public abstract string FileName { get; }
        
        public abstract string VersionsServerPath { get; }

        protected Version LatestVersion =>
            _latestVersion ?? (_latestVersion = VersionUtilities.GetLatestFileVersion(VersionsServerPath));

        [CanBeNull]
        protected Version CurrentVersion => !File.Exists(FileName)
            ? null
            : new Version(FileVersionInfo.GetVersionInfo(FileName).FileVersion);

        public void UpdateIfNewVersionExists()
        {
            if (LatestVersion == null)
                return;

            if (CurrentVersion == null || CurrentVersion < LatestVersion)
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