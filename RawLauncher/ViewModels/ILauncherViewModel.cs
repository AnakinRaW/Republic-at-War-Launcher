using RawLauncher.Framework.Games;
using RawLauncher.Framework.Mods;
using RawLauncher.Framework.Server;

namespace RawLauncher.Framework.ViewModels
{
    public interface ILauncherViewModel
    {
        IMod CurrentMod { get; set; }

        IGame Eaw { get; set; }

        IGame BaseGame { get; set; }

        IHostServer HostServer { get; set; }

        IServer SessionServer { get; set; }

        string RestoreDownloadDir { get; set; }

        string UpdateDownloadDir { get; set; }
    }
}