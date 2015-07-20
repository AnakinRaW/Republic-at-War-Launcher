using RawLauncherWPF.Games;
using RawLauncherWPF.Mods;
using RawLauncherWPF.Server;

namespace RawLauncherWPF.ViewModels
{
    public interface ILauncherViewModel
    {
        IMod CurrentMod { get; set; }

        IGame Eaw { get; set; }

        IGame Foc { get; set; }

        IHostServer HostServer { get; set; }

        IServer SessionServer { get; set; }

        string RestoreDownloadDir { get; set; }

        string UpdateDownloadDir { get; set; }
    }
}