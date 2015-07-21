using System.Threading.Tasks;
using System.Windows;
using ModernApplicationFramework.Commands;
using RawLauncherWPF.UI;
using RawLauncherWPF.Utilities;

namespace RawLauncherWPF.ViewModels
{
    public sealed class LanguageViewModel : LauncherPaneViewModel
    {
        private LanguageTypes _selectedLanguage;
        public LanguageViewModel(ILauncherPane pane) : base(pane)
        {
            _selectedLanguage = LanguageTypes.None;
        }

        #region Commands
        public Command<object> ChangeSelectionCommand => new Command<object>(ChangeSelection, CanChangeSelection);

        private void ChangeSelection(object obj)
        {
            AudioHelper.PlayAudio(AudioHelper.Audio.ButtonPress);
            _selectedLanguage = (LanguageTypes) obj;
        }

        private bool CanChangeSelection(object arg)
        {
            return true;
        }

        public Command ChangeLanguageCommand => new Command(ChangeLanguage);

        private void ChangeLanguage()
        {
            MessageBox.Show("f");
        }

        #endregion
    }

    public enum LanguageTypes
    {
        None,
        Dutch,
        English,
        French,
        German,
        Italian,
        Russian,
        Serbian,
        Spanish,
        Swedish,
        Ukrainian
    }

}
