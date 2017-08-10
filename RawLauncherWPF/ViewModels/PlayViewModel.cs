using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Microsoft.Win32;
using ModernApplicationFramework.CommandBase;
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
            get => _currentSessions;
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
            LauncherPane.MainWindowViewModel.LauncherViewModel.CurrentMod.PrepareStart(LauncherPane.MainWindowViewModel.LauncherViewModel.BaseGame);
            LauncherPane.MainWindowViewModel.LauncherViewModel.BaseGame.PlayGame(
                LauncherPane.MainWindowViewModel.LauncherViewModel.CurrentMod);

            LauncherPane.MainWindowViewModel.LauncherViewModel.BaseGame.GameProcessData.PropertyChanged += GameProcessData_PropertyChanged;   
            
            LauncherPane.MainWindowViewModel. LauncherViewModel.HideMainWindow();     
        }

        private void GameProcessData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName != nameof(GameProcessData.IsProcessRunning))
                    return;
                LauncherPane.MainWindowViewModel.LauncherViewModel.CurrentMod.CleanUpAferGame(LauncherPane.MainWindowViewModel.LauncherViewModel.BaseGame);
                LauncherPane.MainWindowViewModel.LauncherViewModel.BaseGame.GameProcessData.PropertyChanged -= GameProcessData_PropertyChanged;

                ThreadUtilities.ThreadSaveShutdown();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        public Command DefreezeCommand => new Command(DefreezeAsync);

        private async void DefreezeAsync()
        {
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

        private static bool CanOrganizeGame()
        {
            return true;
        }

        private static void OrganizeGame()
        {
            Process.Start("http://www.raworganize.com");
        }

        public Command RefreshSessionsCommand => new Command(RefreshSessions);

        private void RefreshSessions()
        {  
            SetCurrentSessionAsync();
        }

        private void SetCurrentSessionAsync()
        {
            CurrentSessions = GetMessage("PlayCurrentSessionWait");
            Task.Factory.StartNew(() => CurrentSessions = LauncherPane.MainWindowViewModel.LauncherViewModel.SessionServer.DownloadString("count.php"));
        }

        public ICommand ToggleFastLaunchCommand => new ObjectCommand(ToggleFastLaunchAsync, CanToogleFastLaunch);

        private static bool CanToogleFastLaunch(object arg)
        {
            return true;
        }

        private async void ToggleFastLaunchAsync(object arg)
        {
            var toggleButton = arg as ToggleButton;
            if (toggleButton == null)
                return;;
            AudioHelper.PlayAudio(AudioHelper.Audio.Checkbox);
            if (toggleButton.IsChecked == true)
                 await LauncherPane.MainWindowViewModel.LauncherViewModel.CreateFastLaunchFileCommand.Execute();
            if (toggleButton.IsChecked == false)
                await LauncherPane.MainWindowViewModel.LauncherViewModel.DeleteFastLaunchFileCommand.Execute();
        }

        #endregion
    }
}
