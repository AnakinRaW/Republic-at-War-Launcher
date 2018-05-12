using System;
using RawLauncher.Framework.Games;
using RawLauncher.Framework.Utilities;

namespace RawLauncher.Framework.Mods
{
    class DummyMod : IMod
    {
        public DummyMod(IGame baseGame)
        {
            ModDirectory = baseGame.GameDirectory + @"\Mods\Republic_at_War\";
            Version = new Version("0.1");
            InstalledLanguage = LanguageTypes.English;
        }

        public string FolderName { get; }
        public string ModDirectory { get; }
        public string Name { get; }
        public Version Version { get; set; }
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
