using System.IO;

namespace RawLauncher.Framework.Defreezer
{
    public class SteamSaveGame : SaveGame
    {
        public SteamSaveGame(string filePath) : base(filePath)
        {
            if (Path.GetExtension(filePath) != ".PetroglyphFoCSave")
                throw new FileFormatException();
        }

        public override string Name => Path.GetFileName(FilePath);
    }
}