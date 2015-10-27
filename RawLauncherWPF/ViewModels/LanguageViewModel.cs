using System;
using System.IO;
using System.Linq;
using System.Windows;
using ModernApplicationFramework.Commands;
using RawLauncherWPF.Mods;
using RawLauncherWPF.UI;
using RawLauncherWPF.Utilities;
using static System.String;

namespace RawLauncherWPF.ViewModels
{
    public sealed class LanguageViewModel : LauncherPaneViewModel
    {
        public LanguageViewModel(ILauncherPane pane) : base(pane)
        {
            SelectedLanguage = LanguageTypes.None;
            ExternalSupportedLanguages = LanguageTypes.German | LanguageTypes.English;
        }

        private LanguageTypes ExternalSupportedLanguages { get; }
        private string MessageToShowAfterChange { get; set; }
        private LanguageTypes SelectedLanguage { get; set; }

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
            if (File.Exists(mod.ModDirectory + @"Data\Text\MasterTextFile_" + SelectedLanguage + ".dat"))
            {
                MessageToShowAfterChange += "MasterTextFile_" + SelectedLanguage +
                                            " already existed. Skipped it's renaming.\n";
                return;
            }

            var file = Directory.EnumerateFiles(mod.ModDirectory + @"Data\Text\", "MasterTextFile_*").First();

            try
            {
                File.Move(file,
                    mod.ModDirectory + @"Data\Text\MasterTextFile_" + SelectedLanguage.ToString().ToUpper() + ".dat");
            }
            catch (Exception)
            {
                MessageToShowAfterChange += "Could not change MasterTextFile";
            }
        }

        private void ChangeSpeechFolderName(IMod mod)
        {
            if (!Directory.Exists(mod.ModDirectory + @"Data\Audio\Speech\"))
                return;
            if (!Directory.EnumerateDirectories(mod.ModDirectory + @"Data\Audio\Speech", "*").Any())
                return;
            if (Directory.Exists(mod.ModDirectory + @"Data\Audio\Speech\" + SelectedLanguage))
            {
                MessageToShowAfterChange += "A correct named Speech directory already existed. Skipped it's renaming.\n";
                return;
            }

            var directory = Directory.EnumerateDirectories(mod.ModDirectory + @"Data\Audio\Speech", "*").First();
            try
            {
                Directory.Move(directory, mod.ModDirectory + @"Data\Audio\Speech\" + SelectedLanguage);
            }
            catch (Exception)
            {
                MessageToShowAfterChange += "Could not change Speech folder";
            }
        }

        private void ChangeSpeechMegFile(IMod mod)
        {
            if (!Directory.Exists(mod.ModDirectory + @"\Data\"))
                return;
            if (!Directory.EnumerateFiles(mod.ModDirectory + @"\Data\", "*Speech.meg").Any())
                return;

            var languageAlias = CreateAliasLanguage(SelectedLanguage);
            if (File.Exists(mod.ModDirectory + @"\Data\" + languageAlias + "Speech.meg"))
            {
                MessageToShowAfterChange += languageAlias + "Speech.meg already existed. Skipped it's renaming.\n";
                return;
            }
            var file = Directory.EnumerateFiles(mod.ModDirectory + @"\Data\", "*Speech.meg").First();
            try
            {
                File.Move(file, mod.ModDirectory + @"\Data\" + languageAlias + "Speech.meg");
            }
            catch (Exception)
            {
                MessageToShowAfterChange += "Could not change Speech.meg";
            }
        }

        private bool CheckAlreadyInstalledLanguage(IMod mod)
        {
            if (!File.Exists(mod.ModDirectory + @"Data\Text\MasterTextFile_" + SelectedLanguage + ".dat"))
                return false;

            return Directory.Exists(mod.ModDirectory + @"\Data\Audio\Speech\" + SelectedLanguage) &&
                   File.Exists(mod.ModDirectory + @"\Data\" + CreateAliasLanguage(SelectedLanguage) + "Speech.meg");
        }

        private void InternalChangeLanguage(IMod mod)
        {
            if (CheckAlreadyInstalledLanguage(mod))
                MessageToShowAfterChange = SelectedLanguage + " is already installed";
            else
            {
                ChangeMasterTextFile(mod);
                ChangeSpeechMegFile(mod);
                ChangeSpeechFolderName(mod);
                MessageToShowAfterChange += "Successfuly Changed Language";
            }
            MessageProvider.Show(MessageToShowAfterChange);
            MessageToShowAfterChange = Empty;
        }

        #region Commands

        public Command<object> ChangeSelectionCommand => new Command<object>(ChangeSelection, CanChangeSelection);

        private void ChangeSelection(object obj)
        {
            AudioHelper.PlayAudio(AudioHelper.Audio.ButtonPress);
            SelectedLanguage = (LanguageTypes) obj;
        }

        private bool CanChangeSelection(object arg)
        {
            return true;
        }

        public Command ChangeLanguageCommand => new Command(ChangeLanguage);

        private void ChangeLanguage()
        {
            AudioHelper.PlayAudio(AudioHelper.Audio.ButtonPress);
            if (SelectedLanguage == LanguageTypes.None)
            {
                MessageProvider.Show("Select a language, you want to use.");
                return;
            }
            if ((SelectedLanguage & ExternalSupportedLanguages) != 0)
                MessageProvider.Show(
                    "There is also a separate specific Version available for this language. Please check Moddb.com for a Language-Pack or check the RaW installer for more options.");
            MessageProvider.Show(
                "Note that chaging the language in this case means that any [MISSING]s and missing audio will be replaced with the default english version. For any language packs consider our Moddb page or check the Republic at War installer for language options",
                "Repuvlic at War Launcher", MessageBoxButton.OK, MessageBoxImage.Information);
            InternalChangeLanguage(LauncherPane.MainWindowViewModel.LauncherViewModel.CurrentMod);
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
        Ukrainian = 1024
    }
}