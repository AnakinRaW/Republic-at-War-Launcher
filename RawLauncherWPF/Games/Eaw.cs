using System.IO;

namespace RawLauncherWPF.Games
{
    public class Eaw : Game
    {
        public Eaw()
        {
        }

        public Eaw(string gameDirectory) : base(gameDirectory)
        {
        }

        public override string Name => "Empire at War";

        public override Game FindGame()
        {
            if (!File.Exists(Directory.GetParent(Directory.GetCurrentDirectory()) + @"\Star Wars Empire at War\GameData\sweaw.exe"))
                throw new GameExceptions(Name + " does not exists");
            return new Foc(Directory.GetCurrentDirectory());
        }
    }
}
