using System.Threading.Tasks;
using System.Windows.Input;

namespace RawLauncher.Framework.Screens.UpdateScreen
{
    public interface IUpdateScreen : ILauncherScreen
    {
        ICommand OpenChangelogCommand { get; }

        ICommand UpdateModCommand { get; }

        Task<UpdateRestoreStatus> PerformUpdate();
    }
}
