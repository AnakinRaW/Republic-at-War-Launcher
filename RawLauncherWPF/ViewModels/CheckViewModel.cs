using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using ModernApplicationFramework.Commands;
using RawLauncherWPF.ExtensionClasses;
using RawLauncherWPF.Games;
using RawLauncherWPF.Properties;
using RawLauncherWPF.Server;
using RawLauncherWPF.UI;
using RawLauncherWPF.Utilities;
using RawLauncherWPF.Xml;
using static System.String;
using static RawLauncherWPF.Helpers.CheckModHelper;
using static RawLauncherWPF.Utilities.FileUtilities;
using static RawLauncherWPF.Utilities.IndicatorImagesHelper;
using static RawLauncherWPF.Utilities.ProgressBarUtilities;
using static RawLauncherWPF.Utilities.MessageProvider;

namespace RawLauncherWPF.ViewModels
{
    public sealed class CheckViewModel : LauncherPaneViewModel
    {
        private const string CheckFileFileName = "CheckModFileContainer.xml";
        private ImageSource _gameFoundIndicator;
        private string _gameFoundMessage;
        private string _gamesPatched;
        private ImageSource _gamesPatchedIndicator;
        private string _modAi;
        private ImageSource _modAiIndicator;
        private string _modFiles;
        private ImageSource _modFilesIndicator;
        private ImageSource _modFoundIndicator;
        private string _modFoundMessage;
        private int _progress;

        CancellationTokenSource m_source;

        public CheckViewModel(ILauncherPane pane) : base(pane)
        {
            GameFoundIndicator = SetColor(IndicatorColor.Blue);
            ModFoundIndicator = SetColor(IndicatorColor.Blue);
            GamesPatchedIndicator = SetColor(IndicatorColor.Blue);
            ModAiIndicator = SetColor(IndicatorColor.Blue);
            ModFilesIndicator = SetColor(IndicatorColor.Blue);
            CheckFileStream = Stream.Null;
        }

        private IHostServer HostServer => LauncherPane.MainWindowViewModel.LauncherViewModel.HostServer;
        private List<FileContainerFolder> AiFolderList { get; set; }
        private Stream CheckFileStream { get; set; }
        private FileContainer FileContainer { get; set; }
        private List<FileContainerFolder> ModFolderList { get; set; }

        private async Task<bool> CheckFolderList(List<FileContainerFolder> folderList, List<string> excludeList)
        {
            var listToCheck = FileContainerFolder.ListFromExcludeList(folderList, excludeList);
            var result = true;
            var i = 100/listToCheck.Count;

            foreach (var folder in listToCheck)
            {
                try
                {
                    var referenceDir = GetReferenceDir(folder, LauncherPane);
                    if (!await Task.Run(() => folder.Check(referenceDir), m_source.Token))
                        result = false;
                    Debug.WriteLine(referenceDir);
                    await AnimateProgressBar(Progress, Progress + i + 1, 1, this, x => x.Progress);
                }
                catch (TaskCanceledException)
                {
                    result = false;
                }         
            }
            return result;
        }

        private void PrepareForCheck()
        {
            Progress = 0;
            GameFoundIndicator = SetColor(IndicatorColor.Blue);
            ModFoundIndicator = SetColor(IndicatorColor.Blue);
            GamesPatchedIndicator = SetColor(IndicatorColor.Blue);
            ModAiIndicator = SetColor(IndicatorColor.Blue);
            ModFilesIndicator = SetColor(IndicatorColor.Blue);

            IsBlocking = true;
            IsWorking = true;

            GameFoundMessage = Empty;
            ModFoundMessage = Empty;
            GamesPatchedMessage = Empty;
            ModAiMessage = Empty;
            ModFilesMessage = Empty;
        }

        private void PreReturn()
        {
            IsWorking = false;
            IsBlocking = false;
            CheckFileStream = Stream.Null;
            FileContainer = null;
        }

        #region IPropChanged Properties

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

        public string ModAiMessage
        {
            get { return _modAi; }
            set
            {
                if (Equals(value, _modAi))
                    return;
                _modAi = value;
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

        public string ModFilesMessage
        {
            get { return _modFiles; }
            set
            {
                if (Equals(value, _modFiles))
                    return;
                _modFiles = value;
                OnPropertyChanged();
            }
        }

        public ImageSource ModFilesIndicator
        {
            get { return _modFilesIndicator; }
            set
            {
                if (Equals(value, _modFilesIndicator))
                    return;
                _modFilesIndicator = value;
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

        #endregion

        #region PatchGames

        private void CreatePatchMessage(bool eaw, bool foc)
        {

            if (eaw && foc)
                Show("Games successfuly patched.");
            else if (!eaw && !foc)
                Show("Games not successfuly patched.");
            else if (!eaw)
                Show("Foc successfuly patched.\r\nEaw not successfuly patched.");
            else
                Show("Foc not successfuly patched\r\nEaw successfuly patched");
        }

        private bool PatchGame(IGame game)
        {
            return game.Patch();
        }

        #endregion

        #region FindGame

        private async Task<bool> CheckGameExists()
        {
            if (!await CheckFocExistsTask())
            {
                FocNotExistsTasks();
                return false;
            }
            FocExistsTasks();
            return true;
        }

        private async Task<bool> CheckFocExistsTask()
        {
            await AnimateProgressBar(Progress, 101, 10, this, x => x.Progress);
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

        private async Task<bool> CheckModExists()
        {
            if (!await CheckModExistsTask())
            {
                ModNotExistsTasks();
                return false;
            }
            ModExistsTasks();
            return true;
        }

        private async Task<bool> CheckModExistsTask()
        {
            await AnimateProgressBar(Progress, 101, 10, this, x => x.Progress);
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

        private async Task<bool> CheckGamePatched()
        {
            if (!await CheckGameUpdatesInstalled())
            {
                GamesNotUpdated();
                return false;
            }
            GamesUpdated();
            return true;
        }

        private async Task<bool> CheckGameUpdatesInstalled()
        {
            var a = LauncherPane.MainWindowViewModel.LauncherViewModel.Eaw.IsPatched();
            await AnimateProgressBar(Progress, 51, 10, this, x => x.Progress);
            var b = LauncherPane.MainWindowViewModel.LauncherViewModel.Foc.IsPatched();
            await AnimateProgressBar(Progress, 101, 10, this, x => x.Progress);
            return a && b;
        }

        private void GamesNotUpdated()
        {
            GamesPatchedIndicator = SetColor(IndicatorColor.Red);
            GamesPatchedMessage = "games not patched";
            PreReturn();
            Show("You need to update your games. Please press the 'patch' button.");
        }

        private void GamesUpdated()
        {
            GamesPatchedIndicator = SetColor(IndicatorColor.Green);
            GamesPatchedMessage = "games patched";
        }

        #endregion

        #region CheckAI

        private async Task<bool> CheckAiCorrect()
        {
            if (!await CheckFolderList(AiFolderList, new List<string> { @"\Data\CustomMaps\"}))
            {
                AiWrongInstalled();
                return false;
            }
            AiCorrectInstalled();
            return true;
        }

        private void AiCorrectInstalled()
        {
            ModAiIndicator = SetColor(IndicatorColor.Green);
            ModAiMessage = "ai correct";
        }

        private void AiWrongInstalled()
        {
            ModAiIndicator = SetColor(IndicatorColor.Red);
            ModAiMessage = "ai wrong";
            PreReturn();
        }

        #endregion

        #region CheckMod

        private async Task<bool> CheckModCorrect()
        {
            var excludeList = new List<string> {@"XML\Enum\", @"Audio\Speech\*", @"\", @"Text\"};
            if (!await CheckFolderList(ModFolderList, excludeList))
            {
                ModWrongInstalled();
                return false;
            }
            ModCorrectInstalled();
            return true;
        }

        private void ModCorrectInstalled()
        {
            ModFilesIndicator = SetColor(IndicatorColor.Green);
            ModFilesMessage = "mod correct";
        }

        private void ModWrongInstalled()
        {
            ModFilesIndicator = SetColor(IndicatorColor.Red);
            ModFilesMessage = "mod wrong";
            PreReturn();
        }

        #endregion

        #region PrepareXml

        private async Task<bool> PrepareXmlForCheck()
        {
            if (!await Task.FromResult(LoadCheckFileStream()))
                return false;

            if (!await Task.FromResult(ParseXmlFile()))
                return false;
            return true;
        }

        private bool LoadCheckFileStream()
        {
            if (!HostServer.IsRunning())
                GetOffline();
            else
                GetOnline();

            if (CheckFileStream.Length == 0 || CheckFileStream == Stream.Null)
            {
                ModCheckError(null);
                return false;
            }
            var validator = new XmlValidator(Resources.FileContainer.ToStream());

            if (!validator.Validate(CheckFileStream))
            {
                ModCheckError(
                    "The necessary files are not valid. It was also not possible to check them with our server. Please click Restore-Tab and let the launcher redownload the Files.");
                return false;
            }
            return true;
        }

        private bool ParseXmlFile()
        {
            var parser = new XmlObjectParser<FileContainer>(CheckFileStream);
            FileContainer = parser.Parse();

            if (FileContainer.Version != LauncherPane.MainWindowViewModel.LauncherViewModel.CurrentMod.Version)
            {
                ModCheckError(
                    "The Version of the mod does not match to the reference file. Please click Restore-Tab and let the launcher redownload the Files.");
                return false;
            }


            AiFolderList = FileContainer.GetFoldersOfType(TargetType.Ai);
            ModFolderList = FileContainer.GetFoldersOfType(TargetType.Mod);
            return true;
        }

        private void GetOffline()
        {
            if (!Directory.Exists(RestorePathGenerator(false, LauncherPane)) || !File.Exists(RestorePathGenerator(false, LauncherPane) + CheckFileFileName))
            {
                ModCheckError(
                    "Could not find the necessary files to check your version. It was also not possible to check them with our server. Please click Restore-Tab and let the launcher redownload the Files.");
                return;
            }
            CheckFileStream = FileToStream(RestorePathGenerator(false, LauncherPane) + CheckFileFileName);
        }

        private void GetOnline()
        {
            CheckFileStream = HostServer.DownloadString(RestorePathGenerator(true, LauncherPane) + CheckFileFileName).ToStream();
        }

        private void ModCheckError(string message)
        {
            ModAiIndicator = SetColor(IndicatorColor.Red);
            ModAiMessage = "could not check";
            ModFilesIndicator = SetColor(IndicatorColor.Red);
            ModFilesMessage = "could not check";
            PreReturn();
            if (!IsNullOrEmpty(message))
                Show(message);
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

        private void CheckVersion()
        {
            m_source = new CancellationTokenSource();
            PerformCheck();
            
        }

        public Command CancelCommand => new Command(Cancel);

        private void Cancel()
        {
            m_source?.Cancel(false);
        }

        #endregion

        private async void PerformCheck()
        {
            PrepareForCheck();

            //Game exists
            if (!await CheckGameExists())
                return;
            await ThreadUtilities.SleepThread(750);
            await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);

            if (m_source.IsCancellationRequested)
            {
                PreReturn();
                return;
            }
                

            //Mod exists
            if (!await CheckModExists())
                return;
            await ThreadUtilities.SleepThread(750);
            await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);

            //Games patched
            if (!await CheckGamePatched())
                return;
            await ThreadUtilities.SleepThread(750);
            await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);


            if (!await PrepareXmlForCheck())
                return;

            if (!await CheckAiCorrect())
                return;

            await ThreadUtilities.SleepThread(750);
            await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);

            if (!await CheckModCorrect())
                return;

            PreReturn();
        }
    }
}