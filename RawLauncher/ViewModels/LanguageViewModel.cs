using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Input.Base;
using ModernApplicationFramework.Input.Command;
using RawLauncher.Framework.Mods;
using RawLauncher.Framework.UI;
using RawLauncher.Framework.Utilities;

namespace RawLauncher.Framework.ViewModels
{
    public sealed class LanguageViewModel : LauncherPaneViewModel
    {
        private string _customLanguage;

        private string LanguageString => SelectedLanguage.HasFlag(LanguageTypes.Custom)
            ? CustomLanguage
            : SelectedLanguage.ToString();

        private LanguageTypes ExternalSupportedLanguages { get; }
        private string MessageToShowAfterChange { get; set; }
        private LanguageTypes SelectedLanguage { get; set; }

        public string CustomLanguage
        {
            get => _customLanguage;
            set
            {
                if (_customLanguage == value)
                    return;
                _customLanguage = value;
                OnPropertyChanged();
            }
        }

        public LanguageViewModel(ILauncherPane pane) : base(pane)
        {
            SelectedLanguage = LanguageTypes.None;
            ExternalSupportedLanguages = LanguageTypes.German | LanguageTypes.English;
        }

        private static LanguageTypes CreateAliasLanguage(LanguageTypes type)
        {
            return (type &
                    (LanguageTypes.Dutch | LanguageTypes.Serbian | LanguageTypes.Swedish | LanguageTypes.Ukrainian)) !=
                   0
                ? LanguageTypes.English
                : type;
        }

        private void ChangeMasterTextFile(IMod mod)
        {
            if (!Directory.Exists(mod.ModDirectory + @"Data\Text\"))
                return;
            if (!Directory.EnumerateFiles(mod.ModDirectory + @"Data\Text\", "MasterTextFile_*").Any())
                return;
            if (File.Exists(mod.ModDirectory + @"Data\Text\MasterTextFile_" + LanguageString + ".dat"))
            {
                MessageToShowAfterChange += "MasterTextFile_" + LanguageString +
                                            " already existed. Skipped it's renaming.\n";
                return;
            }

            var file = Directory.EnumerateFiles(mod.ModDirectory + @"Data\Text\", "MasterTextFile_*").First();

            try
            {
                File.Move(file,
                    mod.ModDirectory + @"Data\Text\MasterTextFile_" + LanguageString.ToUpper() + ".dat");
            }
            catch (Exception)
            {
                MessageToShowAfterChange += MessageProvider.GetMessage("LanguageMessageTextRenameFailed");
            }
        }

        private void ChangeSpeechFolderName(IMod mod)
        {
            if (!Directory.Exists(mod.ModDirectory + @"Data\Audio\Speech\"))
                return;
            if (!Directory.EnumerateDirectories(mod.ModDirectory + @"Data\Audio\Speech", "*").Any())
                return;
            if (Directory.Exists(mod.ModDirectory + @"Data\Audio\Speech\" + LanguageString))
                return;

            var directory = Directory.EnumerateDirectories(mod.ModDirectory + @"Data\Audio\Speech", "*").First();
            try
            {
                Directory.Move(directory, mod.ModDirectory + @"Data\Audio\Speech\" + LanguageString);
            }
            catch (Exception)
            {
                MessageToShowAfterChange += MessageProvider.GetMessage("LanguageMessageSpeechRenameFailed");
            }
        }

        private void ChangeSpeechMegFile(IMod mod)
        {
            if (!Directory.Exists(mod.ModDirectory + @"\Data\"))
                return;
            if (!Directory.EnumerateFiles(mod.ModDirectory + @"\Data\", "*Speech.meg").Any())
                return;

            var languageAlias = CreateAliasLanguage(SelectedLanguage);

            if (languageAlias == LanguageTypes.Custom)
            {
                if (File.Exists(mod.ModDirectory + @"\Data\" + CustomLanguage + "Speech.meg"))
                    return;
            }
            else
            {
                if (File.Exists(mod.ModDirectory + @"\Data\" + languageAlias + "Speech.meg"))
                    return;
            }

            if (File.Exists(mod.ModDirectory + @"\Data\" + languageAlias + "Speech.meg"))
                return;
            var file = Directory.EnumerateFiles(mod.ModDirectory + @"\Data\", "*Speech.meg").First();
            try
            {
                File.Move(file, mod.ModDirectory + @"\Data\" + languageAlias + "Speech.meg");
            }
            catch (Exception)
            {
                MessageToShowAfterChange += MessageProvider.GetMessage("LanguageMessageSpeechFileRenameFailed");
            }
        }

        private bool CheckAlreadyInstalledLanguage(IMod mod)
        {
            if (!File.Exists(mod.ModDirectory + @"Data\Text\MasterTextFile_" + LanguageString + ".dat"))
                return false;

            return Directory.Exists(mod.ModDirectory + @"\Data\Audio\Speech\" + LanguageString) &&
                   File.Exists(mod.ModDirectory + @"\Data\" + CreateAliasLanguage(SelectedLanguage) + "Speech.meg");
        }

        private void InternalChangeLanguage(IMod mod, bool showMessage = false)
        {
            if (!CheckAlreadyInstalledLanguage(mod))
            {
                ChangeMasterTextFile(mod);
                ChangeSpeechMegFile(mod);
                ChangeSpeechFolderName(mod);
            }
            if (showMessage)
            {
                MessageToShowAfterChange += MessageProvider.GetMessage("LanguageMessageChangedSuccess");
                MessageProvider.Show(MessageToShowAfterChange);
            }
            MessageToShowAfterChange = string.Empty;
        }

        #region Commands

        public ICommand ChangeSelectionCommand => new DelegateCommand(ChangeSelection, CanChangeSelection);

        private void ChangeSelection(object obj)
        {
            AudioHelper.PlayAudio(AudioHelper.Audio.ButtonPress);
            SelectedLanguage = (LanguageTypes) obj;
        }

        private bool CanChangeSelection(object arg)
        {
            return true;
        }

        public ICommand ChangeLanguageCommand => new DelegateCommand(ChangeLanguage, CanChangeLanguage);

        private bool CanChangeLanguage(object _)
        {
            if (SelectedLanguage == LanguageTypes.Custom && string.IsNullOrEmpty(CustomLanguage))
                return false;
            return true;
        }

        private void ChangeLanguage(object _)
        {
            if (SelectedLanguage == LanguageTypes.None)
            {
                MessageProvider.Show(MessageProvider.GetMessage("LanguageNoneSelected"));
                return;
            }
            if ((SelectedLanguage & ExternalSupportedLanguages) != 0)
                MessageProvider.Show(MessageProvider.GetMessage("LanguageAdditionalSupport"));
            MessageProvider.Show(MessageProvider.GetMessage("LangugeOperationQuestion"),
                "Republic at War Launcher", MessageBoxButton.OK, MessageBoxImage.Information);
            InternalChangeLanguage(LauncherPane.MainWindowViewModel.LauncherViewModel.CurrentMod, true);
        }

        public void ChangeLanguage(LanguageTypes language, bool showMesage = false)
        {
            if (language == LanguageTypes.None)
                return;
            SelectedLanguage = language;
            InternalChangeLanguage(LauncherPane.MainWindowViewModel.LauncherViewModel.CurrentMod, showMesage);
        }

        #endregion
    }

    [Flags]
    public enum LanguageTypes
    {
        None = 1,
        Dutch = 2,
        English = 4,
        French = 8,
        German = 16,
        Italian = 32,
        Russian = 64,
        Serbian = 128,
        Spanish = 265,
        Swedish = 512,
        Ukrainian = 1024,
        Custom = 2048
    }
}