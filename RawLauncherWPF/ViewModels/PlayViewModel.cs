using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using Microsoft.Win32;
using ModernApplicationFramework.Commands;
using RawLauncherWPF.Defreezer;
using RawLauncherWPF.UI;
using RawLauncherWPF.Utilities;
using static RawLauncherWPF.NativeMethods.NativeMethods;

namespace RawLauncherWPF.ViewModels
{
    public sealed class PlayViewModel : LauncherPaneViewModel
    {

        private string _currentSessions;

        public PlayViewModel(ILauncherPane pane) : base(pane)
        {
            if (ComputerHasInternetConnection())
                SetCurrentSessionAsync();           
        }

        public string CurrentSessions
        {
            get
            {
                return _currentSessions;
            }
            set
            {
                if (Equals(value, _currentSessions))
                    return;
                _currentSessions = value;
                OnPropertyChanged();
            }
        }

        #region Command

        public Command PlayModCommand => new Command(PlayMod, CanPlayMod);

        private bool CanPlayMod()
        {
            return true;
        }

        private void PlayMod()
        {
            AudioHelper.PlayAudio(AudioHelper.Audio.Play);
            Thread.Sleep(1100);
            // TODO: Shuffle .txts if they are irrelevant for MP
            LauncherPane.MainWindowViewModel.LauncherViewModel.Foc.PlayGame(
                LauncherPane.MainWindowViewModel.LauncherViewModel.CurrentMod);
            Application.Current.Shutdown();
        }

        public Command DefreezeCommand => new Command(Defreeze);

        private async void Defreeze()
        {
            AudioHelper.PlayAudio(AudioHelper.Audio.ButtonPress);
            var oFd = new OpenFileDialog
            {
                InitialDirectory = LauncherPane.MainWindowViewModel.LauncherViewModel.Foc.SaveGameDirectory,
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

        public Command OrganizeGameCommand => new Command(OrganizeGame, CanOrganizeGame);

        private bool CanOrganizeGame()
        {
            return true;
        }

        private void OrganizeGame()
        {
            AudioHelper.PlayAudio(AudioHelper.Audio.ButtonPress);
            Process.Start("http://www.raworganize.com");
        }

        public Command RefreshSessionsCommand => new Command(RefreshSessions);

        private void RefreshSessions()
        {
            AudioHelper.PlayAudio(AudioHelper.Audio.ButtonPress);      
            SetCurrentSessionAsync();
        }

        private void SetCurrentSessionAsync()
        {
            CurrentSessions = "Wait..";
            Task.Factory.StartNew(() => CurrentSessions = LauncherPane.MainWindowViewModel.LauncherViewModel.SessionServer.DownloadString("count.php"));
        }

        public Command<ToggleButton> ToggleFastLaunchCommand => new Command<ToggleButton>(ToggleFastLaunch, CanToogleFastLaunch);

        private bool CanToogleFastLaunch(ToggleButton arg)
        {
            return true;
        }

        async private void ToggleFastLaunch(ToggleButton arg)
        {
            AudioHelper.PlayAudio(AudioHelper.Audio.Checkbox);
            if (arg.IsChecked == true)
                 await LauncherPane.MainWindowViewModel.LauncherViewModel.CreateFastLaunchFileCommand.Execute();
            if (arg.IsChecked == false)
                await LauncherPane.MainWindowViewModel.LauncherViewModel.DeleteFastLaunchFileCommand.Execute();
        }

        #endregion
    }
}
