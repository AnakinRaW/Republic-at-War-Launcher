using System;
using System.IO;
using System.Linq;
using System.Windows;
using RawLauncher.Framework.Mods;
using RawLauncher.Framework.Utilities;

namespace RawLauncher.Framework.Games
{
    public abstract class AbstractFocGame : IGame
    {
        public string GameDirectory { get; protected set; }

        public abstract string SaveGameDirectory { get; }

        public abstract string Name { get; }

        public GameProcessData GameProcessData { get; }

        protected abstract string GameExeFileName { get; }

        protected abstract int DefaultXmlFileCount { get; }

        protected AbstractFocGame()
        {
            
        }

        protected AbstractFocGame(string gameDirectory)
        {
            GameDirectory = gameDirectory;
            if (!Exists())
                throw new GameExceptions(MessageProvider.GetMessage("ExceptionGameExist"));
            GameProcessData = new GameProcessData();
        }

        public bool Exists() => File.Exists(Path.Combine(GameDirectory, GameExeFileName));

        public void PlayGame()
        {
            throw new NotImplementedException();
        }

        public abstract void PlayGame(IMod mod);

        public abstract bool Patch();

        public abstract bool IsPatched();

        public void DeleteMod(IMod mod)
        {
            if (mod == null)
            {
                MessageBox.Show("Republic at War was not found");
                return;
            }
            if (mod is DummyMod)
                return;
            ClearDataFolder();
            Patch();
            mod.Delete();
        }

        public void ClearDataFolder()
        {
            var customMapsPath = Path.Combine(Path.Combine(GameDirectory, @"Data\CustomMaps\"));
            var scriptsPath = Path.Combine(Path.Combine(GameDirectory, @"Data\Scripts\"));
            var xmlPath = Path.Combine(Path.Combine(GameDirectory, @"Data\XML\"));


            if (Directory.Exists(customMapsPath))
                FileUtilities.DeleteDirectory(customMapsPath);
            if (Directory.Exists(scriptsPath))
                FileUtilities.DeleteDirectory(scriptsPath);
            if (Directory.Exists(xmlPath))
                FileUtilities.DeleteDirectory(xmlPath);
        }

        public bool IsGameAiClear()
        {
            if (Directory.Exists(Path.Combine(GameDirectory, @"Data\Scripts\")))
                return false;
            var xmlDir = Path.Combine(GameDirectory, @"Data\XML\");
            if (!Directory.Exists(xmlDir))
                return false;
            var number = Directory.EnumerateFiles(xmlDir).Count();
            if (number != DefaultXmlFileCount)
                return false;
            if (Directory.Exists(Path.Combine(xmlDir, @"AI\")))
                return false;
            return true;
        }
    }
}
