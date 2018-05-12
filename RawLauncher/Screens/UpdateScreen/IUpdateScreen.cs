using System.Threading.Tasks;
using System.Windows.Input;

namespace RawLauncher.Framework.Screens.UpdateScreen
{
    public interface IUpdateScreen : ILauncherScreen, IUpdateRestoreBase
    {
        ICommand OpenChangelogCommand { get; }

        ICommand UpdateModCommand { get; }

        Task<UpdateRestoreStatus> PerformUpdate();
    }
}
