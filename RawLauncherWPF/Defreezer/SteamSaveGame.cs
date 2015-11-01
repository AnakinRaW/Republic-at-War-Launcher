using System.IO;

namespace RawLauncherWPF.Defreezer
{
    internal class SteamSaveGame : SaveGame
    {
        public SteamSaveGame(string filePath) : base(filePath)
        {
            if (Path.GetExtension(filePath) != ".PetroglyphFoCSave")
                throw new FileFormatException();
        }

        public override string Name => Path.GetFileName(FilePath);
    }
}