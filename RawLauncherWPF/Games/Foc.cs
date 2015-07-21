using System;
using System.Diagnostics;
using System.IO;
using RawLauncherWPF.Mods;

namespace RawLauncherWPF.Games
{
    public class Foc : IGame
    {
        public Foc()
        {
        }

        public Foc(string gameDirectory)
        {
            GameDirectory = gameDirectory;
            if (!Exists())
                throw new GameExceptions("This Game does not exists");
        }

        public string GameDirectory { get; }

        public string Name => "Forces of Corruption";

        public bool Exists() => Directory.Exists(GameDirectory);

        public IGame FindGame()
        {
            if (!File.Exists(Directory.GetCurrentDirectory() + @"\swfoc.exe"))
                throw new GameExceptions(Name + " does not exists");
            return new Foc(Directory.GetCurrentDirectory());
        }

        public void PlayGame()
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = GameDirectory + @"\swfoc.exe",
                    WorkingDirectory = GameDirectory,
                    UseShellExecute = false
                }
            };
            try
            {
                process.Start();
            }
            catch (Exception)
            {
                //ignored
            }
        }

        public void PlayGame(IMod mod)
        {
            if (!mod.Exists())
                throw new ModExceptions("Mod does not exists anymore.");
            if (!mod.ModDirectory.StartsWith(GameDirectory))
                throw new ModExceptions("Mod is not compatible");
            if (!mod.HasCorrectFolderStructure)
                throw new ModExceptions("Mod is not correct installed");

            var process = new Process
            {
                StartInfo =
                {
                    FileName = GameDirectory + @"\swfoc.exe",
                    Arguments = "MODPATH=" + mod.LaunchArgumentPath,
                    WorkingDirectory = GameDirectory,
                    UseShellExecute = false
                }
            };
            try
            {
                process.Start();
            }
            catch (Exception)
            {
                //ignored
            }
        }
    }
}
