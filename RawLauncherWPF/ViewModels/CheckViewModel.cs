using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ModernApplicationFramework.Commands;
using RawLauncherWPF.ExtensionClasses;
using RawLauncherWPF.Games;
using RawLauncherWPF.Properties;
using RawLauncherWPF.UI;
using RawLauncherWPF.Utilities;
using RawLauncherWPF.Xml;
using static System.String;
using static RawLauncherWPF.Utilities.IndicatorImagesHelper;
using static RawLauncherWPF.Utilities.ProgressBarUtilities;

namespace RawLauncherWPF.ViewModels
{
    public sealed class CheckViewModel : LauncherPaneViewModel
    {
        private ImageSource _gameFoundIndicator;
        private string _gameFoundMessage;
        private string _gamesPatched;
        private ImageSource _gamesPatchedIndicator;
        private string _modAiCorrect;
        private ImageSource _modAiIndicator;
        private ImageSource _modFoundIndicator;
        private string _modFoundMessage;
        private string _modXmlCorrect;
        private ImageSource _modXmlIndicator;
        private int _progress;

        public CheckViewModel(ILauncherPane pane) : base(pane)
        {
            GameFoundIndicator = SetColor(IndicatorColor.Blue);
            ModFoundIndicator = SetColor(IndicatorColor.Blue);
            GamesPatchedIndicator = SetColor(IndicatorColor.Blue);
            ModAiIndicator = SetColor(IndicatorColor.Blue);
            ModXmlIndicator = SetColor(IndicatorColor.Blue);
        }

        public ImageSource GameFoundIndicator
        {
            get { return _gameFoundIndicator; }
            set
            {
                if (Equals(value, _gameFoundIndicator))
                    return;
                _gameFoundIndicator = value;
                OnPropertyChanged();
            }
        }

        public string GameFoundMessage
        {
            get { return _gameFoundMessage; }
            set
            {
                if (Equals(value, _gameFoundMessage))
                    return;
                _gameFoundMessage = value;
                OnPropertyChanged();
            }
        }

        public ImageSource GamesPatchedIndicator
        {
            get { return _gamesPatchedIndicator; }
            set
            {
                if (Equals(value, _gamesPatchedIndicator))
                    return;
                _gamesPatchedIndicator = value;
                OnPropertyChanged();
            }
        }

        public string GamesPatchedMessage
        {
            get { return _gamesPatched; }
            set
            {
                if (Equals(value, _gamesPatched))
                    return;
                _gamesPatched = value;
                OnPropertyChanged();
            }
        }

        public string ModAiCorrectMessage
        {
            get { return _modAiCorrect; }
            set
            {
                if (Equals(value, _modAiCorrect))
                    return;
                _modAiCorrect = value;
                OnPropertyChanged();
            }
        }

        public ImageSource ModAiIndicator
        {
            get { return _modAiIndicator; }
            set
            {
                if (Equals(value, _modAiIndicator))
                    return;
                _modAiIndicator = value;
                OnPropertyChanged();
            }
        }

        public ImageSource ModFoundIndicator
        {
            get { return _modFoundIndicator; }
            set
            {
                if (Equals(value, _modFoundIndicator))
                    return;
                _modFoundIndicator = value;
                OnPropertyChanged();
            }
        }

        public string ModFoundMessage
        {
            get { return _modFoundMessage; }
            set
            {
                if (Equals(value, _modFoundMessage))
                    return;
                _modFoundMessage = value;
                OnPropertyChanged();
            }
        }

        public string ModXmlCorrectMessage
        {
            get { return _modXmlCorrect; }
            set
            {
                if (Equals(value, _modXmlCorrect))
                    return;
                _modXmlCorrect = value;
                OnPropertyChanged();
            }
        }

        public ImageSource ModXmlIndicator
        {
            get { return _modXmlIndicator; }
            set
            {
                if (Equals(value, _modXmlIndicator))
                    return;
                _modXmlIndicator = value;
                OnPropertyChanged();
            }
        }

        public int Progress
        {
            get { return _progress; }
            set
            {
                if (Equals(value, _progress))
                    return;
                _progress = value;
                OnPropertyChanged();
            }
        }

        async private Task CheckOffline()
        {
            if (!Directory.Exists(LauncherPane.MainWindowViewModel.LauncherViewModel.RestoreDownloadDir))
            {
                await OfflineFilesNotFound();
                return;
            }
            if (!File.Exists(LauncherPane.MainWindowViewModel.LauncherViewModel.RestoreDownloadDir + @"CheckModFileContainer.xml"))
            {
                await OfflineFilesNotFound();
                return;
            }

            var validator = new XmlValidator(Resources.FileContainer.ToStream());

            if (!validator.Validate(LauncherPane.MainWindowViewModel.LauncherViewModel.RestoreDownloadDir +
                                    @"CheckModFileContainer.xml"))
            {
                await OfflineFilesNotValid();
                return;
            }

            var fileContainer =
                new XmlObjectParser<FileContainer>(
                    LauncherPane.MainWindowViewModel.LauncherViewModel.RestoreDownloadDir + @"CheckModFileContainer.xml")
                    .Parse();

            if (fileContainer.Version != LauncherPane.MainWindowViewModel.LauncherViewModel.CurrentMod.Version)
            {
                await OfflineVersionNotEqual();
                return;
            }

            //////////////  Fill Data   /////////////////
            var aiFolders = new List<FileContainerFolder>();
            var modFolders = new List<FileContainerFolder>();

            foreach (var folder in fileContainer.Folders)
            {
                if (folder.TargetType == TargetType.Ai)
                    aiFolders.Add(folder);
                if (folder.TargetType == TargetType.Mod)
                    modFolders.Add(folder);
            }
            //////////////  Fill Data   /////////////////

            //////////////  AI Check   /////////////////
            bool aiResult = true;
            foreach (var fileContainerFolder in aiFolders)
            {
                if (!CheckFolder(fileContainerFolder))
                    aiResult = false;
            }
            if (!aiResult)
            {
                //TODO: Wrong AI 
                return;
            }
            //////////////  AI Check   /////////////////

        }

        private bool CheckFolder(FileContainerFolder folder)
        {
            var rootDir = folder.TargetType == TargetType.Ai
                ? LauncherPane.MainWindowViewModel.LauncherViewModel.Foc.GameDirectory
                : LauncherPane.MainWindowViewModel.LauncherViewModel.CurrentMod.ModDirectory;

            if (!Directory.Exists(rootDir + folder.TargetPath))
            {
                MessageBox.Show("Fail");
                return false;
            }
            if (Directory.GetFiles(rootDir + folder.TargetPath).Length.ToString() != folder.Count)
            {
                MessageBox.Show("Count Fail");
                return false;
            }
            //TODO: Check Hash

            return true;
        }

        async private Task OfflineVersionNotEqual()
        {
            ModAiIndicator = SetColor(IndicatorColor.Red);
            ModAiCorrectMessage = "cout not check";
            await AnimateProgressBar(Progress, 100, 10, this, x => x.Progress);
            MessageBox.Show(
                "The Version of the mod does not match to the reference file. Please click Restore-Tab and let the launcher redownload the Files.");
        }

        async private Task OfflineFilesNotValid()
        {
            ModAiIndicator = SetColor(IndicatorColor.Red);
            ModAiCorrectMessage = "cout not check";
            await AnimateProgressBar(Progress, 100, 10, this, x => x.Progress);
            MessageBox.Show(
                "The necessary files are not valid. It was also not possible to check them with our server. Please click Restore-Tab and let the launcher redownload the Files.");
        }

        #region OfflineCheck
        
        async private Task OfflineFilesNotFound()
        {
            ModAiIndicator = SetColor(IndicatorColor.Red);
            ModAiCorrectMessage = "cout not check";
            await AnimateProgressBar(Progress, 100, 10, this, x => x.Progress);
            MessageBox.Show(
                "Could not find the necessary files to check your version. It was also not possible to check them with our server. Please click Restore-Tab and let the launcher redownload the Files.");
        }

        #endregion

        private void CheckOnline()
        {
        }

        private void CreatePatchMessage(bool eaw, bool foc)
        {
            if (eaw && foc)
                MessageBox.Show("Games successfuly patched.");
            else if (!eaw && !foc)
                MessageBox.Show("Games not successfuly patched.");
            else if (!eaw)
                MessageBox.Show("Foc successfuly patched.\r\nEaw not successfuly patched.");
            else
                MessageBox.Show("Foc not successfuly patched\r\nEaw successfuly patched");
        }

        private bool PatchGame(IGame game)
        {
            return game.Patch();
        }

        private void PrepareForCheck()
        {
            Progress = 0;
            GameFoundIndicator = SetColor(IndicatorColor.Blue);
            ModFoundIndicator = SetColor(IndicatorColor.Blue);
            GamesPatchedIndicator = SetColor(IndicatorColor.Blue);
            ModAiIndicator = SetColor(IndicatorColor.Blue);
            ModXmlIndicator = SetColor(IndicatorColor.Blue);

            GameFoundMessage = Empty;
            ModFoundMessage = Empty;
            GamesPatchedMessage = Empty;
            ModAiCorrectMessage = Empty;
            ModXmlCorrectMessage = Empty;
        }

        private void PreReturn()
        {
            IsWorking = false;
            IsBlocking = false;
        }

        #region FindGame

        async private Task CheckGameExists()
        {
            if (!await CheckFocExistsTask())
            {
                FocNotExistsTasks();
                return;
            }
            FocExistsTasks();
        }

        private async Task<bool> CheckFocExistsTask()
        {
            await AnimateProgressBar(Progress, 100, 10, this, x => x.Progress);
            return LauncherPane.MainWindowViewModel.LauncherViewModel.Foc.Exists();
        }

        private void FocNotExistsTasks()
        {
            GameFoundIndicator = SetColor(IndicatorColor.Red);
            GameFoundMessage = "foc not found";
            PreReturn();
        }

        private void FocExistsTasks()
        {
            GameFoundIndicator = SetColor(IndicatorColor.Green);
            GameFoundMessage = "foc found";
        }

        #endregion

        #region FindMod

        async private Task CheckModExists()
        {
            if (!await CheckModExistsTask())
            {
                ModNotExistsTasks();
                return;
            }
            ModExistsTasks();
        }

        async private Task<bool> CheckModExistsTask()
        {
            await AnimateProgressBar(Progress, 100, 10, this, x => x.Progress);
            return LauncherPane.MainWindowViewModel.LauncherViewModel.CurrentMod.Exists();
        }

        private void ModNotExistsTasks()
        {
            ModFoundIndicator = SetColor(IndicatorColor.Red);
            ModFoundMessage = "raw not found";
            PreReturn();
        }

        private void ModExistsTasks()
        {
            ModFoundIndicator = SetColor(IndicatorColor.Green);
            ModFoundMessage = "raw found";
        }

        #endregion

        #region CheckPatch

        async private Task CheckGamePatched()
        {
            if (!await CheckGameUpdatesInstalled())
            {
                GamesNotUpdated();
                return;
            }
            GamesUpdated();
        }

        async private Task<bool> CheckGameUpdatesInstalled()
        {
            var a = LauncherPane.MainWindowViewModel.LauncherViewModel.Eaw.IsPatched();
            await AnimateProgressBar(Progress, 50, 10, this, x => x.Progress);
            var b = LauncherPane.MainWindowViewModel.LauncherViewModel.Eaw.IsPatched();
            await AnimateProgressBar(Progress, 100, 10, this, x => x.Progress);
            return a && b;
        }

        private void GamesNotUpdated()
        {
            GamesPatchedIndicator = SetColor(IndicatorColor.Red);
            GamesPatchedMessage = "games not patched";
            MessageBox.Show("You need to update your games. Please press the 'patch' button.");
            PreReturn();
        }

        private void GamesUpdated()
        {
            GamesPatchedIndicator = SetColor(IndicatorColor.Green);
            GamesPatchedMessage = "games patched";
        }

        #endregion

        #region Commands

        public Command PatchGamesCommand => new Command(PatchGames);

        private void PatchGames()
        {
            AudioHelper.PlayAudio(AudioHelper.Audio.ButtonPress);
            var eaw = PatchGame(LauncherPane.MainWindowViewModel.LauncherViewModel.Eaw);
            var foc = PatchGame(LauncherPane.MainWindowViewModel.LauncherViewModel.Foc);
            CreatePatchMessage(eaw, foc);
        }

        public Command CheckVersionCommand => new Command(CheckVersion);

        private async void CheckVersion()
        {
            IsBlocking = true;
            IsWorking = true;

            PrepareForCheck();

            //Game exists
            //await CheckGameExists();
            
            //await ThreadUtilities.SleepThread(750);
            //await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);

            ////Mod exists
            //await CheckModExists();
            
            //await ThreadUtilities.SleepThread(750);
            //await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);

            ////Games patched
            //await CheckGamePatched();
            
            //await ThreadUtilities.SleepThread(750);
            //await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);

            //Check Mod online/offline
            if (!await LauncherPane.MainWindowViewModel.LauncherViewModel.HostServer.CheckRunningAsync())
                await CheckOffline();
            else
                //TODO: Change
                await CheckOffline();

            PreReturn();
        }

        #endregion
    }
}