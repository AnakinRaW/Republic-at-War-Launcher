using System.IO;
using System.Windows;
using RawLauncherWPF.Mods;

namespace RawLauncherWPF.Games
{
    public class Eaw : IGame
    {
        public Eaw()
        {
        }

        public Eaw(string gameDirectory)
        {
            GameDirectory = gameDirectory;
            if (!Exists())
                throw new GameExceptions("This Game does not exists");
        }

        public string GameDirectory { get; }

        public string Name => "Empire at War";

        public bool Exists() => Directory.Exists(GameDirectory);

        public IGame FindGame()
        {
            if (!File.Exists(Directory.GetParent(Directory.GetCurrentDirectory()) + @"\Star Wars Empire at War\GameData\sweaw.exe"))
                throw new GameExceptions(Name + " does not exists");
            return new Eaw(Directory.GetParent(Directory.GetCurrentDirectory()) + @"\Star Wars Empire at War\GameData\");
        }

        public void PlayGame()
        {
            //Ignored
        }

        public void PlayGame(IMod mod)
        {
            //Ignored
        }
    }
}
