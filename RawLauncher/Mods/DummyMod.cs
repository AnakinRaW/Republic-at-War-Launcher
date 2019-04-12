using System;
using RawLauncher.Framework.Games;
using RawLauncher.Framework.Utilities;
using RawLauncher.Framework.Versioning;

namespace RawLauncher.Framework.Mods
{
    internal class DummyMod : IMod
    {
        public const string VersionName = "0-NotInstalled";

        public DummyMod(IGame baseGame)
        {
            ModDirectory = baseGame.GameDirectory + @"\Mods\Republic_at_War\";
            Version = new ModVersion(VersionName);
            InstalledLanguage = LanguageTypes.English;
        }
        public string FolderName { get; }
        public string ModDirectory { get; }
        public string Name { get; }
        public ModVersion Version { get; set; }
        public LanguageTypes InstalledLanguage { get; }
        public bool WorkshopMod => false;

        public bool Exists()
        {
            throw new NotImplementedException();
        }

        public IMod FindMod(IGame baseGame)
        {
            throw new NotImplementedException();
        }

        public void PrepareStart(IGame game)
        {
            throw new NotImplementedException();
        }

        public void CleanUpAferGame(IGame game)
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }
    }
}
