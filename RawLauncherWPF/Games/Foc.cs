using System.IO;

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
    }
}
