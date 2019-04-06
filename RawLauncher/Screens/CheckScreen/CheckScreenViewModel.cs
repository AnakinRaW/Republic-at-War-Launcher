using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ModernApplicationFramework.Input.Command;
using RawLauncher.Framework.Configuration;
using RawLauncher.Framework.ExtensionClasses;
using RawLauncher.Framework.Launcher;
using RawLauncher.Framework.Models;
using RawLauncher.Framework.Server;
using RawLauncher.Framework.Utilities;
using RawLauncher.Framework.Versioning;
using RawLauncher.Framework.Xml;

namespace RawLauncher.Framework.Screens.CheckScreen
{
    [Export(typeof(ILauncherScreen))]
    [Export(typeof(ICheckScreen))]
    public class CheckScreenViewModel : LauncherScreen, ICheckScreen
    {
        private const string CheckFileFileName = "CheckModFileContainer.xml";

        private readonly LauncherModel _launcher;
        private readonly IHostServer _hostServer;
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
        private CancellationTokenSource _mSource;
        private double _progress;
        private string _progressStatus;
        private readonly MessageRecorder _messageRecorder;

        public ICommand PatchGamesCommand => new Command(PatchGames);

        public ICommand CheckVersionCommand => new Command(CheckVersion);

        public ICommand CancelCommand => new Command(Cancel);

        public ImageSource GameFoundIndicator
        {
            get => _gameFoundIndicator;
            set
            {
                if (Equals(value, _gameFoundIndicator))
                    return;
                _gameFoundIndicator = value;
                NotifyOfPropertyChange();
            }
        }

        public string GameFoundMessage
        {
            get => _gameFoundMessage;
            set
            {
                if (Equals(value, _gameFoundMessage))
                    return;
                _gameFoundMessage = value;
                NotifyOfPropertyChange();
            }
        }

        public ImageSource GamesPatchedIndicator
        {
            get => _gamesPatchedIndicator;
            set
            {
                if (Equals(value, _gamesPatchedIndicator))
                    return;
                _gamesPatchedIndicator = value;
                NotifyOfPropertyChange();
            }
        }

        public string GamesPatchedMessage
        {
            get => _gamesPatched;
            set
            {
                if (Equals(value, _gamesPatched))
                    return;
                _gamesPatched = value;
                NotifyOfPropertyChange();
            }
        }

        public string ModAiMessage
        {
            get => _modAi;
            set
            {
                if (Equals(value, _modAi))
                    return;
                _modAi = value;
                NotifyOfPropertyChange();
            }
        }

        public ImageSource ModAiIndicator
        {
            get => _modAiIndicator;
            set
            {
                if (Equals(value, _modAiIndicator))
                    return;
                _modAiIndicator = value;
                NotifyOfPropertyChange();
            }
        }

        public ImageSource ModFoundIndicator
        {
            get => _modFoundIndicator;
            set
            {
                if (Equals(value, _modFoundIndicator))
                    return;
                _modFoundIndicator = value;
                NotifyOfPropertyChange();
            }
        }

        public string ModFoundMessage
        {
            get => _modFoundMessage;
            set
            {
                if (Equals(value, _modFoundMessage))
                    return;
                _modFoundMessage = value;
                NotifyOfPropertyChange();
            }
        }

        public string ModFilesMessage
        {
            get => _modFiles;
            set
            {
                if (Equals(value, _modFiles))
                    return;
                _modFiles = value;
                NotifyOfPropertyChange();
            }
        }

        public ImageSource ModFilesIndicator
        {
            get => _modFilesIndicator;
            set
            {
                if (Equals(value, _modFilesIndicator))
                    return;
                _modFilesIndicator = value;
                NotifyOfPropertyChange();
            }
        }

        public double Progress
        {
            get => _progress;
            set
            {
                if (Equals(value, _progress))
                    return;
                _progress = value;
                NotifyOfPropertyChange();
            }
        }

        public string ProzessStatus
        {
            get => _progressStatus;
            set
            {
                if (Equals(value, _progressStatus))
                    return;
                _progressStatus = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Contains AI Folder Information
        /// </summary>
        private List<FileContainerFolder> AiFolderList { get; set; }

        /// <summary>
        /// Stream which contains the XML data
        /// </summary>
        private Stream CheckFileStream { get; set; }

        /// <summary>
        /// Container with all information extracted from XML File/Stream
        /// </summary>
        private FileContainer FileContainer { get; set; }

        /// <summary>
        /// Contains Mod Folder Information
        /// </summary>
        private List<FileContainerFolder> ModFolderList { get; set; }

        [ImportingConstructor]
        public CheckScreenViewModel(LauncherModel launcher, IHostServer hostServer)
        {
            _launcher = launcher;
            _hostServer = hostServer;
            CheckFileStream = Stream.Null;
            _messageRecorder = new MessageRecorder();

            GameFoundIndicator = IndicatorImagesHelper.SetColor(IndicatorImagesHelper.IndicatorColor.Blue);
            ModFoundIndicator = IndicatorImagesHelper.SetColor(IndicatorImagesHelper.IndicatorColor.Blue);
            GamesPatchedIndicator = IndicatorImagesHelper.SetColor(IndicatorImagesHelper.IndicatorColor.Blue);
            ModAiIndicator = IndicatorImagesHelper.SetColor(IndicatorImagesHelper.IndicatorColor.Blue);
            ModFilesIndicator = IndicatorImagesHelper.SetColor(IndicatorImagesHelper.IndicatorColor.Blue);
        }

        public void Cancel()
        {
            _mSource?.Cancel(false);
        }

        private async Task<bool> CheckFolderListAsync(List<FileContainerFolder> folderList, IReadOnlyCollection<string> excludeList)
        {
            var listToCheck = FileContainerFolder.ListFromExcludeList(folderList, excludeList);
            var result = true;
            var i = (double)100 / listToCheck.Count;

            foreach (var folder in listToCheck)
            {
                try
                {
                    var referenceDir = CheckModHelper.GetReferenceDir(folder);
                    var error = CheckResult.None;
                    if (await Task.Run(() => error = folder.Check(referenceDir), _mSource.Token) != CheckResult.None)
                    {
                        var errorMessage = CreateErrorMessage(error, referenceDir);
                        _messageRecorder.AppandMessage(errorMessage);
                        result = false;
                    }

                    var path = new DirectoryInfo(referenceDir).Parent?.Parent?.Name + @"\" +
                               new DirectoryInfo(referenceDir).Parent?.Name + @"\" +
                               new DirectoryInfo(referenceDir).Name;

                    ProzessStatus = MessageProvider.GetMessage("CheckStatusChecking", path);
                    Debug.WriteLine(referenceDir);
                    Progress = Progress + i;
                }
                catch (TaskCanceledException)
                {
                    result = false;
                }
            }
            return result;
        }

        private string CreateErrorMessage(CheckResult error, string referenceDir)
        {
            switch (error)
            {
                case CheckResult.Count:
                    return MessageProvider.GetMessage("CheckFolderNotValidCount", referenceDir);
                case CheckResult.Exist:
                    return MessageProvider.GetMessage("CheckFolderNotValidExists", referenceDir);
                case CheckResult.Hash:
                    return MessageProvider.GetMessage("CheckFolderNotValidHash", referenceDir);
                case CheckResult.None:
                    return String.Empty;
                default:
                    throw new ArgumentOutOfRangeException(nameof(error), error, null);
            }
        }

        private void CreatePatchMessage(bool eaw, bool foc)
        {
            if (eaw && foc)
                MessageProvider.Show(MessageProvider.GetMessage("CheckPatchGamesSuccess"));
            else if (!eaw && !foc)
                MessageProvider.Show(MessageProvider.GetMessage("CheckPatchGamesFailed"));
            else if (!eaw)
                MessageProvider.Show(MessageProvider.GetMessage("CheckPatchGamesFailedEaw"));
            else
                MessageProvider.Show(MessageProvider.GetMessage("CheckPatchGamesFailedBase"));
        }

        private async void PerformCheckAsync()
        {
            _mSource = new CancellationTokenSource();
            PrepareUi();
            _messageRecorder.Flush();

            //Game exists
            ProzessStatus = MessageProvider.GetMessage("CheckStatusCheckingGameExist");
            if (!await CheckGameExistsAsync())
                return;
            await ThreadUtilities.SleepThread(250);
            await ProgressBarUtilities.AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);

            if (_mSource.IsCancellationRequested)
            {
                ResetUi();
                return;
            }


            //Mod exists
            ProzessStatus = MessageProvider.GetMessage("CheckStatusCheckingModExist");
            if (!await CheckModExistsAsync())
                return;
            await ThreadUtilities.SleepThread(250);
            await ProgressBarUtilities.AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);

            //Games patched
            ProzessStatus = MessageProvider.GetMessage("CheckStatusCheckingGamePatches");
            if (!await CheckGamePatchedAsync())
                return;
            await ThreadUtilities.SleepThread(250);
            await ProgressBarUtilities.AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);

            //Prepare XML-based Check
            ProzessStatus = MessageProvider.GetMessage("CheckStatusPrepareAiModCheck");
            if (!await PrepareXmlForCheckAsync())
                return;

            //Check AI
            if (!await CheckAiCorrectAsync())
                return;

            await ThreadUtilities.SleepThread(250);
            await ProgressBarUtilities.AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);

            //Check Mod
            if (!await CheckModCorrectAsync())
                return;

            ResetUi();
        }

        private void PrepareUi()
        {
            Progress = 0;
            GameFoundIndicator = IndicatorImagesHelper.SetColor(IndicatorImagesHelper.IndicatorColor.Blue);
            ModFoundIndicator = IndicatorImagesHelper.SetColor(IndicatorImagesHelper.IndicatorColor.Blue);
            GamesPatchedIndicator = IndicatorImagesHelper.SetColor(IndicatorImagesHelper.IndicatorColor.Blue);
            ModAiIndicator = IndicatorImagesHelper.SetColor(IndicatorImagesHelper.IndicatorColor.Blue);
            ModFilesIndicator = IndicatorImagesHelper.SetColor(IndicatorImagesHelper.IndicatorColor.Blue);

            IsBlocking = true;
            IsWorking = true;

            GameFoundMessage = String.Empty;
            ModFoundMessage = String.Empty;
            GamesPatchedMessage = String.Empty;
            ModAiMessage = String.Empty;
            ModFilesMessage = String.Empty;
        }

        private void ResetUi()
        {
            IsWorking = false;
            IsBlocking = false;
            CheckFileStream = Stream.Null;
            FileContainer = null;
        }

        private async Task<bool> CheckGameExistsAsync()
        {
            if (!await CheckFocExistsTaskAsync())
            {
                FocNotExistsTasks();
                return false;
            }
            FocExistsTasks();
            return true;
        }

        private async Task<bool> CheckFocExistsTaskAsync()
        {
            await ProgressBarUtilities.AnimateProgressBar(Progress, 101, 10, this, x => x.Progress);
            return _launcher.BaseGame.Exists();
        }

        private void FocNotExistsTasks()
        {
            GameFoundIndicator = IndicatorImagesHelper.SetColor(IndicatorImagesHelper.IndicatorColor.Red);
            GameFoundMessage = MessageProvider.GetMessage("CheckMessageGameNotFound");
            ResetUi();
        }

        private void FocExistsTasks()
        {
            GameFoundIndicator = IndicatorImagesHelper.SetColor(IndicatorImagesHelper.IndicatorColor.Green);
            GameFoundMessage = MessageProvider.GetMessage("CheckMessageGameFound");
        }

        private async Task<bool> CheckModExistsAsync()
        {
            if (!await CheckModExistsTaskAsync())
            {
                ModNotExistsTasks();
                return false;
            }
            ModExistsTasks();
            return true;
        }

        private async Task<bool> CheckModExistsTaskAsync()
        {
            await ProgressBarUtilities.AnimateProgressBar(Progress, 101, 10, this, x => x.Progress);
            if (_launcher.CurrentMod == null)
                return false;
            return _launcher.CurrentMod.Exists();
        }

        private void ModNotExistsTasks()
        {
            ModFoundIndicator = IndicatorImagesHelper.SetColor(IndicatorImagesHelper.IndicatorColor.Red);
            ModFoundMessage = MessageProvider.GetMessage("CheckMessageModNotFound");
            ResetUi();
        }

        private void ModExistsTasks()
        {
            ModFoundIndicator = IndicatorImagesHelper.SetColor(IndicatorImagesHelper.IndicatorColor.Green);
            ModFoundMessage = MessageProvider.GetMessage("CheckMessageModFound");
        }

        private async Task<bool> CheckGamePatchedAsync()
        {
            if (!await CheckGameUpdatesInstalledAsync())
            {
                GamesNotUpdated();
                return false;
            }
            GamesUpdated();
            return true;
        }

        private async Task<bool> CheckGameUpdatesInstalledAsync()
        {
            var a = _launcher.Eaw.IsPatched();
            await ProgressBarUtilities.AnimateProgressBar(Progress, 51, 10, this, x => x.Progress);
            var b = _launcher.BaseGame.IsPatched();
            await ProgressBarUtilities.AnimateProgressBar(Progress, 101, 10, this, x => x.Progress);
            return a && b;
        }

        private void GamesNotUpdated()
        {
            GamesPatchedIndicator = IndicatorImagesHelper.SetColor(IndicatorImagesHelper.IndicatorColor.Red);
            GamesPatchedMessage = MessageProvider.GetMessage("CheckMessageGamesNotPatched");
            ResetUi();
            MessageProvider.Show(MessageProvider.GetMessage("CheckGamesNotPatchedMessage"));
        }

        private void GamesUpdated()
        {
            GamesPatchedIndicator = IndicatorImagesHelper.SetColor(IndicatorImagesHelper.IndicatorColor.Green);
            GamesPatchedMessage = MessageProvider.GetMessage("CheckMessageGamesPatched");
        }

        private async Task<bool> CheckAiCorrectAsync()
        {
            var flag = true;
            if (_launcher.CurrentMod.Version >= ModVersion.Parse("1.1.5.1"))
            {
                var aiPath = Path.Combine(_launcher.BaseGame.GameDirectory, @"Data\XML\AI\");

                if (Directory.Exists(aiPath))
                    if (Directory.EnumerateFileSystemEntries(aiPath).Any())
                    {
                        _messageRecorder.AppandMessage("XML folder contains additional AI data");
                        flag = false;
                    }
                var scriptsPath = Path.Combine(_launcher.BaseGame.GameDirectory, @"Data\Scripts\");
                if (Directory.Exists(scriptsPath))
                    if (Directory.EnumerateFileSystemEntries(scriptsPath).Any())
                    {
                        _messageRecorder.AppandMessage("Scripts folder contains additional data");
                        flag = false;
                    }
            }
            if (!flag)
            {
                AiWrongInstalled();
                if (!_mSource.Token.IsCancellationRequested)
                {
                    var result = MessageProvider.Show(MessageProvider.GetMessage("CheckAIFolderNotValid"), "Republic at War", MessageBoxButton.YesNo,
                        MessageBoxImage.Error, MessageBoxResult.Yes);
                    if (result == MessageBoxResult.Yes)
                        _messageRecorder.Save(MessageProvider.GetMessage("CheckFolderNotValid"));
                }   
                return false;
            }
            if (!await CheckFolderListAsync(AiFolderList, new List<string> { @"\Data\CustomMaps\" }))
            {
                AiWrongInstalled();
                var result = MessageProvider.Show(MessageProvider.GetMessage("CheckAIFolderNotValid"), "Republic at War", MessageBoxButton.YesNo,
                    MessageBoxImage.Error, MessageBoxResult.Yes);
                if (result == MessageBoxResult.Yes)
                    _messageRecorder.Save(MessageProvider.GetMessage("CheckFolderNotValid"));
                return false;
            }
            AiCorrectInstalled();
            return true;
        }

        private void AiCorrectInstalled()
        {
            ModAiIndicator = IndicatorImagesHelper.SetColor(IndicatorImagesHelper.IndicatorColor.Green);
            ModAiMessage = MessageProvider.GetMessage("CheckMessageAiCorrect");
        }

        private void AiWrongInstalled()
        {
            ModAiIndicator = IndicatorImagesHelper.SetColor(IndicatorImagesHelper.IndicatorColor.Red);
            ModAiMessage = _mSource.Token.IsCancellationRequested ? MessageProvider.GetMessage("CheckMessageAborted") : MessageProvider.GetMessage("CheckMessageAiWrong");
            ResetUi();
        }

        private async Task<bool> CheckModCorrectAsync()
        {

            var excludeList = CheckModHelper.BuildExcludeList(_launcher.CurrentMod.Version);

            if (!await CheckFolderListAsync(ModFolderList, excludeList))
            {
                ModWrongInstalled();
                if (!_mSource.Token.IsCancellationRequested)
                {
                    var result = MessageProvider.Show(MessageProvider.GetMessage("CheckModFolderNotValid"), "Republic at War", MessageBoxButton.YesNo,
                        MessageBoxImage.Error, MessageBoxResult.Yes);
                    if (result == MessageBoxResult.Yes)
                        _messageRecorder.Save(MessageProvider.GetMessage("CheckFolderNotValid"));
                }
                return false;
            }
            ModCorrectInstalled();
            return true;
        }

        private void ModCorrectInstalled()
        {
            ModFilesIndicator = IndicatorImagesHelper.SetColor(IndicatorImagesHelper.IndicatorColor.Green);
            ModFilesMessage = MessageProvider.GetMessage("CheckMessageModCorrect");
        }

        private void ModWrongInstalled()
        {
            ModFilesIndicator = IndicatorImagesHelper.SetColor(IndicatorImagesHelper.IndicatorColor.Red);
            ModFilesMessage = _mSource.Token.IsCancellationRequested ? MessageProvider.GetMessage("CheckMessageAborted") : MessageProvider.GetMessage("CheckMessageModWrong");
            ResetUi();
        }

        private async Task<bool> PrepareXmlForCheckAsync()
        {
            if (!await LoadCheckFileStreamAsync())
                return false;

            if (!await Task.FromResult(ParseXmlFile()))
                return false;
            return true;
        }

        private async Task<bool> LoadCheckFileStreamAsync()
        {
            if (!_hostServer.IsRunning())
                GetOffline();
            else
            {
                await GetOnlineAsync();
                WriteOnlineDataToDisk();
            }

            if (CheckFileStream.IsEmpty())
            {
                ModCheckError(null);
                return false;
            }
            var validator = new XmlValidator(Properties.Resources.FileContainer.ToStream());

            if (!validator.Validate(CheckFileStream))
            {
                ModCheckError(MessageProvider.GetMessage("CheckXmlValidateError"));
                return false;
            }
            return true;
        }

        /// <summary>
        /// Writes XML-Stream to local file for offline check
        /// </summary>
        private void WriteOnlineDataToDisk()
        {
            if (CheckFileStream.IsEmpty())
                return;
            CheckFileStream.ToFile(_launcher.GetRescueFilePath(CheckFileFileName, false));
        }

        /// <summary>
        /// Parses the XML-Stream, checks the Version of it and fills the Containers
        /// </summary>
        /// <returns></returns>
        private bool ParseXmlFile()
        {
            var parser = new XmlObjectParser<FileContainer>(CheckFileStream);
            FileContainer = parser.Parse();

            if (FileContainer.Version != _launcher.CurrentMod.Version)
            {
                ModCheckError(MessageProvider.GetMessage("CheckXmlWrongVersion"));
                return false;
            }

            AiFolderList = FileContainer.GetFoldersOfType(TargetType.Ai);
            ModFolderList = FileContainer.GetFoldersOfType(TargetType.Mod);
            return true;
        }

        /// <summary>
        /// Tries to get local Restore XML
        /// </summary>
        private void GetOffline()
        {
            if (!VersionUtilities.GetAllAvailableModVersionsOffline().Contains(_launcher.CurrentMod.Version))
            {
                MessageProvider.Show(MessageProvider.GetMessage("CheckVersionNotFound"));
                return;
            }
            if (!File.Exists(_launcher.GetRescueFilePath(CheckFileFileName, false)))
            {
                ModCheckError(MessageProvider.GetMessage("CheckOfflineXmlNotFound"));
                return;
            }
            CheckFileStream = FileUtilities.FileToStream(_launcher.GetRescueFilePath(CheckFileFileName, false));
        }

        /// <summary>
        /// Tries to get online Restore XML
        /// </summary>
        private async Task GetOnlineAsync()
        {
            if (!VersionUtilities.GetAllAvailableModVersionsOnline().Contains(_launcher.CurrentMod.Version))
            {
                MessageProvider.Show(MessageProvider.GetMessage("CheckVersionNotFound"));
                return;
            }
            await
                Task.Factory.StartNew(
                    () =>
                        CheckFileStream =
                            _hostServer.DownloadString(_launcher.GetRescueFilePath(CheckFileFileName, true))
                                .ToStream());
        }

        private void ModCheckError(string message)
        {
            ModAiIndicator = IndicatorImagesHelper.SetColor(IndicatorImagesHelper.IndicatorColor.Red);
            ModAiMessage = MessageProvider.GetMessage("CheckMessageCouldNotCheck");
            ModFilesIndicator = IndicatorImagesHelper.SetColor(IndicatorImagesHelper.IndicatorColor.Red);
            ModFilesMessage = MessageProvider.GetMessage("CheckMessageCouldNotCheck");
            ResetUi();
            if (!String.IsNullOrEmpty(message))
                MessageProvider.Show(message);
        }

        private void CheckVersion()
        {
            PerformCheckAsync();
        }

        private void PatchGames()
        {
            var eaw = _launcher.Eaw.Patch();
            if (_launcher.CurrentMod.Version >= ModVersion.Parse("1.1.5.1"))
                _launcher.BaseGame.ClearDataFolder();
            var foc = _launcher.BaseGame.Patch();
            CreatePatchMessage(eaw, foc);
            PerformCheckAsync();
        }
    }
}
