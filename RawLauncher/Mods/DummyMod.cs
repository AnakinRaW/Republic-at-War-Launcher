using System;
using System.IO;
using RawLauncher.Framework.Games;
using RawLauncher.Framework.ViewModels;

namespace RawLauncher.Framework.Mods
{
    class DummyMod : IMod
    {

        public DummyMod()
        {
            ModDirectory = Directory.GetCurrentDirectory() + @"\Mods\Republic_at_War\";
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

        public IMod FindMod()
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
