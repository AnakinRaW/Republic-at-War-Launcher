using Caliburn.Micro;

namespace RawLauncher.Framework.Screens
{
    public interface ILauncherScreen : IScreen
    {
        bool IsWorking { get; set; }
        bool IsBlocking { get; set; }
        bool CanExecute { get; set; }
    }
}