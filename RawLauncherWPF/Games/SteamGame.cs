using System;
using RawLauncherWPF.Mods;

namespace RawLauncherWPF.Games
{
    public sealed class SteamGame : IGame
    {
        //TODO: Do class
        public string GameDirectory { get; }
        public string SaveGameDirectory { get; }
        public string Name { get; }
        public bool Exists()
        {
            throw new NotImplementedException();
        }

        public IGame FindGame()
        {
            throw new NotImplementedException();
        }

        public void PlayGame()
        {
            throw new NotImplementedException();
        }

        public void PlayGame(IMod mod)
        {
            throw new NotImplementedException();
        }

        public bool Patch()
        {
            throw new NotImplementedException();
        }

        public bool IsPatched()
        {
            throw new NotImplementedException();
        }

        public void DeleteMod(string name)
        {
            throw new NotImplementedException();
        }

        public void ClearDataFolder()
        {
            throw new NotImplementedException();
        }
    }
}
