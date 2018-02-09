using System.Windows.Input;

namespace RawLauncher.Framework.Screens.LanguageScreen
{
    public interface ILanguageScreen : ILauncherScreen
    {
        ICommand ChangeSelectionCommand { get; }

        ICommand ChangeLanguageCommand { get; }

        string CustomLanguage { get; set; }
    }
}
