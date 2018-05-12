using System.Windows.Input;
using RawLauncher.Framework.Utilities;

namespace RawLauncher.Framework.Screens.LanguageScreen
{
    public interface ILanguageScreen : ILauncherScreen, IHasSelection
    {
        ICommand ChangeLanguageCommand { get; }

        string CustomLanguage { get; set; }

        void ChangeLanguage(LanguageTypes language, bool showMesage = false);
    }
}
