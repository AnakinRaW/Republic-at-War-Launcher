using System.Windows.Input;

namespace RawLauncher.Framework.Screens.PlayScreen
{
    public interface IPlayScreen : ILauncherScreen
    {
        ICommand PlayModCommand { get; }

        ICommand DefreezeCommand { get; }

        ICommand ToggleFastLaunchCommand { get; }
    }
}