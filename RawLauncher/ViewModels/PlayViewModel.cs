using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Microsoft.Win32;
using ModernApplicationFramework.Input.Command;
using RawLauncher.Framework.Defreezer;
using RawLauncher.Framework.Mods;
using RawLauncher.Framework.UI;
using RawLauncher.Framework.Utilities;

namespace RawLauncher.Framework.ViewModels
{
    public sealed class PlayViewModel : LauncherPaneViewModel
    {

        private string _currentSessions;

        public PlayViewModel(ILauncherPane pane) : base(pane)
        {
            if (NativeMethods.NativeMethods.ComputerHasInternetConnection())
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
            var bw = new BackgroundWorker();
            bw.DoWork += Bw_DoWork;
            bw.RunWorkerAsync();
           
            LauncherPane.MainWindowViewModel.LauncherViewModel.CurrentMod.PrepareStart(LauncherPane.MainWindowViewModel.LauncherViewModel.BaseGame);
            LauncherPane.MainWindowViewModel.LauncherViewModel.BaseGame.PlayGame(
                LauncherPane.MainWindowViewModel.LauncherViewModel.CurrentMod);

            //LauncherPane.MainWindowViewModel.LauncherViewModel.BaseGame.GameProcessData.PropertyChanged += GameProcessData_PropertyChanged;   

            //LauncherPane.MainWindowViewModel. LauncherViewModel.HideMainWindow();  
            ThreadUtilities.ThreadSaveShutdown();
        }

        private static void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            AudioHelper.PlayAudio(AudioHelper.Audio.Play);
        }

        //private void GameProcessData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    try
        //    {
        //        if (e.PropertyName != nameof(GameProcessData.IsProcessRunning))
        //            return;
        //        LauncherPane.MainWindowViewModel.LauncherViewModel.CurrentMod.CleanUpAferGame(LauncherPane.MainWindowViewModel.LauncherViewModel.BaseGame);
        //        LauncherPane.MainWindowViewModel.LauncherViewModel.BaseGame.GameProcessData.PropertyChanged -= GameProcessData_PropertyChanged;

        //        ThreadUtilities.ThreadSaveShutdown();
        //    }
        //    catch (Exception exception)
        //    {
        //        MessageBox.Show(exception.Message);
        //    }
        //}

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
            MessageProvider.Show("Done");
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
            CurrentSessions = MessageProvider.GetMessage("PlayCurrentSessionWait");
            Task.Factory.StartNew(() => CurrentSessions = LauncherPane.MainWindowViewModel.LauncherViewModel.SessionServer.DownloadString("count.php"));
        }

        public ICommand ToggleFastLaunchCommand => new ObjectCommand(ToggleFastLaunchAsync, CanToogleFastLaunch);

        private static bool CanToogleFastLaunch(object arg)
        {
            return true;
        }

        private async void ToggleFastLaunchAsync(object arg)
        {
            if (!(arg is ToggleButton toggleButton))
                return;
            AudioHelper.PlayAudio(AudioHelper.Audio.Checkbox);
            if (toggleButton.IsChecked == true)
                 await LauncherPane.MainWindowViewModel.LauncherViewModel.CreateFastLaunchFileCommand.Execute();
            if (toggleButton.IsChecked == false)
                await LauncherPane.MainWindowViewModel.LauncherViewModel.DeleteFastLaunchFileCommand.Execute();
        }

        #endregion
    }
}
