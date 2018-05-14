using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Microsoft.Win32;
using ModernApplicationFramework.Input.Command;
using RawLauncher.Framework.Defreezer;
using RawLauncher.Framework.Games;
using RawLauncher.Framework.Launcher;
using RawLauncher.Framework.Mods;
using RawLauncher.Framework.Utilities;
using RawLauncher.Framework.Utilities.Converters;

namespace RawLauncher.Framework.Screens.PlayScreen
{
    [Export(typeof(ILauncherScreen))]
    [Export(typeof(IPlayScreen))]
    public class PlayScreenViewModel : LauncherScreen, IPlayScreen
    {
        private readonly LauncherModel _launcher;
        private string _autosaveButtonText;

        private readonly AutosaveToButtonTextConverter _buttonTextConverter = new AutosaveToButtonTextConverter();

        public ICommand PlayModCommand => new Command(PlayMod);

        public ICommand DefreezeCommand => new Command(DefreezeAsync);

        public ICommand ToggleFastLaunchCommand => new DelegateCommand(ToggleFastLaunchAsync);

        public ICommand AutosaveInfoCommand => new Command(ShowAutosaveInfo);

        public ICommand TriggerAutosaveCommand => new DelegateCommand(TriggerAutosave);

        public string AutosaveButtonText
        {
            get => _autosaveButtonText;
            set
            {
                if (value == _autosaveButtonText)
                    return;
                _autosaveButtonText = value;
                NotifyOfPropertyChange();
            }
        }

        [ImportingConstructor]

        public PlayScreenViewModel(LauncherModel launcher)
        {
            _launcher = launcher;
            switch (_launcher.BaseGame)
            {
                case null:
                    return;
                case SteamGame steamGame:
                    AutosaveButtonText = (string)_buttonTextConverter.Convert(this, typeof(string),
                        !steamGame.AutosaveEnabled,
                        CultureInfo.CurrentCulture);
                    break;
            }
        }

        private void PlayMod()
        {
            var bw = new BackgroundWorker();
            bw.DoWork += Bw_DoWork;
            bw.RunWorkerAsync();
            PlayHelper.Play(_launcher.BaseGame, _launcher.CurrentMod);
        }

        private static void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            AudioPlayer.PlayAudio(AudioPlayer.Audio.Play);
        }

        private async void DefreezeAsync()
        {
            var initDir = _launcher.CurrentMod == null ||
                          _launcher.CurrentMod is DummyMod
                ? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
                : _launcher.BaseGame.SaveGameDirectory;

            var oFd = new OpenFileDialog
            {
                InitialDirectory = initDir,
                Filter = "Savegame Files (*.sav; *.PetroglyphFoCSave) | *.sav; *.PetroglyphFoCSave",
                Title = "Select a Savegame"
            };
            if (oFd.ShowDialog() != true)
                return;
            SaveGame saveGame;
            if (Path.GetExtension(oFd.FileName) == ".sav")
                saveGame = new RetailSaveGame(oFd.FileName);
            else
                saveGame = new SteamSaveGame(oFd.FileName);
            var d = new Defreezer.Defreezer(saveGame);
            await Task.Run(() => d.DefreezeSaveGame());
            MessageProvider.Show("Done");
        }

        private async void ToggleFastLaunchAsync(object arg)
        {
            if (!(arg is ToggleButton toggleButton))
                return;
            AudioPlayer.PlayAudio(AudioPlayer.Audio.Checkbox);
            if (toggleButton.IsChecked == true)
                await _launcher.CreateFastLaunchFileCommand.Execute();
            if (toggleButton.IsChecked == false)
                await _launcher.DeleteFastLaunchFileCommand.Execute();
        }

        private static void ShowAutosaveInfo()
        {
            MessageProvider.ShowInformation(MessageProvider.GetMessage("AutosaveInfoMessage"));
        }

        private void TriggerAutosave(object obj)
        {
            AudioPlayer.PlayAudio(AudioPlayer.Audio.ButtonPress);
            if (_launcher?.BaseGame == null || !(_launcher.BaseGame is SteamGame steamGame))
                return;
            steamGame.SwitchAutosaveEnabledStatus();
            AutosaveButtonText = (string) _buttonTextConverter.Convert(this, typeof(string),
                !steamGame.AutosaveEnabled,
                CultureInfo.CurrentCulture);
        }
    }
}
