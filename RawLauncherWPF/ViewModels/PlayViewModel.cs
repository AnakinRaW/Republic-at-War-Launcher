using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using Microsoft.Win32;
using ModernApplicationFramework.Commands;
using RawLauncherWPF.Defreezer;
using RawLauncherWPF.Games;
using RawLauncherWPF.Mods;
using RawLauncherWPF.UI;
using RawLauncherWPF.Utilities;
using static RawLauncherWPF.NativeMethods.NativeMethods;
using static RawLauncherWPF.Utilities.MessageProvider;

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

        private static bool CanPlayMod()
        {
            return true;
        }

        private void PlayMod()
        {
            AudioHelper.PlayAudio(AudioHelper.Audio.Play);
            Thread.Sleep(1100);
            LauncherPane.MainWindowViewModel.LauncherViewModel.CurrentMod.PrepareStart(LauncherPane.MainWindowViewModel.LauncherViewModel.BaseGame);
            LauncherPane.MainWindowViewModel.LauncherViewModel.BaseGame.PlayGame(
                LauncherPane.MainWindowViewModel.LauncherViewModel.CurrentMod);

            LauncherPane.MainWindowViewModel.LauncherViewModel.BaseGame.GameProcessData.PropertyChanged += GameProcessData_PropertyChanged;   
            
            LauncherPane.MainWindowViewModel. LauncherViewModel.HideMainWindow();     
        }

        private void GameProcessData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
           if (e.PropertyName != nameof(GameProcessData.IsProcessRunning))
                return;
            LauncherPane.MainWindowViewModel.LauncherViewModel.CurrentMod.CleanUpAferGame(LauncherPane.MainWindowViewModel.LauncherViewModel.BaseGame);
            LauncherPane.MainWindowViewModel.LauncherViewModel.BaseGame.GameProcessData.PropertyChanged -= GameProcessData_PropertyChanged;
            Application.Current.Shutdown();
        }

        public Command DefreezeCommand => new Command(Defreeze);

        private async void Defreeze()
        {
            AudioHelper.PlayAudio(AudioHelper.Audio.ButtonPress);

            var initDir = LauncherPane.MainWindowViewModel.LauncherViewModel.CurrentMod == null ||
                             LauncherPane.MainWindowViewModel.LauncherViewModel.CurrentMod is DummyMod
                ? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
                : LauncherPane.MainWindowViewModel.LauncherViewModel.BaseGame.SaveGameDirectory;

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
            Show("Done");
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
            CurrentSessions = GetMessage("PlayCurrentSessionWait");
            Task.Factory.StartNew(() => CurrentSessions = LauncherPane.MainWindowViewModel.LauncherViewModel.SessionServer.DownloadString("count.php"));
        }

        public Command<ToggleButton> ToggleFastLaunchCommand => new Command<ToggleButton>(ToggleFastLaunch, CanToogleFastLaunch);

        private bool CanToogleFastLaunch(ToggleButton arg)
        {
            return true;
        }

        private async void ToggleFastLaunch(ToggleButton arg)
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
