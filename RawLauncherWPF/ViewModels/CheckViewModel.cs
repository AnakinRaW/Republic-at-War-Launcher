using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ModernApplicationFramework.CommandBase;
using RawLauncherWPF.Configuration;
using RawLauncherWPF.ExtensionClasses;
using RawLauncherWPF.Models;
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
        private CancellationTokenSource _mSource;
        private double _progress;
        private string _progressStatus;

        private readonly MessageRecorder _messageRecorder;

        public CheckViewModel(ILauncherPane pane) : base(pane)
        {
            LauncherViewModel = LauncherPane.MainWindowViewModel.LauncherViewModel;
            GameFoundIndicator = SetColor(IndicatorColor.Blue);
            ModFoundIndicator = SetColor(IndicatorColor.Blue);
            GamesPatchedIndicator = SetColor(IndicatorColor.Blue);
            ModAiIndicator = SetColor(IndicatorColor.Blue);
            ModFilesIndicator = SetColor(IndicatorColor.Blue);
            CheckFileStream = Stream.Null;

            _messageRecorder = new MessageRecorder();
        }

        /// <summary>
        /// Reference to the HostServer
        /// </summary>
        private IHostServer HostServer => LauncherViewModel.HostServer;

        private LauncherViewModel LauncherViewModel { get; }

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

        /// <summary>
        /// Performs the actuall Folder comparison and throws error messages
        /// </summary>
        /// <param name="folderList">List where T is FileContainerFolder.</param>
        /// <param name="excludeList">List where T is string. Used to exclude some paths</param>
        /// <returns>Returns if Chekc was successfull or not</returns>
        private async Task<bool> CheckFolderListAsync(List<FileContainerFolder> folderList, List<string> excludeList)
        {
            var listToCheck = FileContainerFolder.ListFromExcludeList(folderList, excludeList);
            var result = true;
            var i = (double) 100/listToCheck.Count;

            foreach (var folder in listToCheck)
            {
                try
                {
                    var referenceDir = GetReferenceDir(folder);
                    var error = CheckResult.None;
                    if (await Task.Run(() => error = folder.Check(referenceDir), _mSource.Token) != CheckResult.None)
                    {
                        var errorMessage = CreateErrorMessage(error, referenceDir);
                        _messageRecorder.AppandMessage(errorMessage);
                        result = false;
                    }
                    ProzessStatus = GetMessage("CheckStatusChecking", Path.GetDirectoryName(referenceDir));
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
                    return GetMessage("CheckFolderNotValidCount", referenceDir);
                case CheckResult.Exist:
                    return GetMessage("CheckFolderNotValidExists", referenceDir);
                case CheckResult.Hash:
                    return GetMessage("CheckFolderNotValidHash", referenceDir);
                case CheckResult.None:
                    return Empty;
                default:
                    throw new ArgumentOutOfRangeException(nameof(error), error, null);
            }
        }

        #region PatchGames

        /// <summary>
        /// Creats a status message whether the games are patches successfully
        /// </summary>
        /// <param name="eaw"></param>
        /// <param name="foc"></param>
        private void CreatePatchMessage(bool eaw, bool foc)
        {
            if (eaw && foc)
                Show(GetMessage("CheckPatchGamesSuccess"));
            else if (!eaw && !foc)
                Show(GetMessage("CheckPatchGamesFailed"));
            else if (!eaw)
                Show(GetMessage("CheckPatchGamesFailedEaw"));
            else
                Show(GetMessage("CheckPatchGamesFailedBase"));
        }

        #endregion

        private async void PerformCheckAsync()
        {
            _mSource = new CancellationTokenSource();
            PrepareUi();
            _messageRecorder.Flush();

            //Game exists
            ProzessStatus = GetMessage("CheckStatusCheckingGameExist");
            if (!await CheckGameExistsAsync())
                return;
            await ThreadUtilities.SleepThread(250);
            await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);

            if (_mSource.IsCancellationRequested)
            {
                ResetUi();
                return;
            }


            //Mod exists
            ProzessStatus = GetMessage("CheckStatusCheckingModExist");
            if (!await CheckModExistsAsync())
                return;
            await ThreadUtilities.SleepThread(250);
            await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);

            //Games patched
            ProzessStatus = GetMessage("CheckStatusCheckingGamePatches");
            if (!await CheckGamePatchedAsync())
                return;
            await ThreadUtilities.SleepThread(250);
            await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);

            //Prepare XML-based Check
            ProzessStatus = GetMessage("CheckStatusPrepareAiModCheck");
            if (!await PrepareXmlForCheckAsync())
                return;

            //Check AI
            if (!await CheckAiCorrectAsync())
                return;

            await ThreadUtilities.SleepThread(250);
            await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);

            //Check Mod
            if (!await CheckModCorrectAsync())
                return;

            ResetUi();
        }

        /// <summary>
        /// Inits all messages and status identifier
        /// </summary>
        private void PrepareUi()
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

        /// <summary>
        /// Resets internal data to init State
        /// </summary>
        private void ResetUi()
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

        public double Progress
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

        public string ProzessStatus
        {
            get { return _progressStatus; }
            set
            {
                if (Equals(value, _progressStatus))
                    return;
                _progressStatus = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region FindGame

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
            await AnimateProgressBar(Progress, 101, 10, this, x => x.Progress);
            return LauncherPane.MainWindowViewModel.LauncherViewModel.BaseGame.Exists();
        }

        private void FocNotExistsTasks()
        {
            GameFoundIndicator = SetColor(IndicatorColor.Red);
            GameFoundMessage = GetMessage("CheckMessageGameNotFound");
            ResetUi();
        }

        private void FocExistsTasks()
        {
            GameFoundIndicator = SetColor(IndicatorColor.Green);
            GameFoundMessage = GetMessage("CheckMessageGameFound");
        }

        #endregion

        #region FindMod

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
            await AnimateProgressBar(Progress, 101, 10, this, x => x.Progress);
            if (LauncherPane.MainWindowViewModel.LauncherViewModel.CurrentMod == null)
                return false;
            return LauncherPane.MainWindowViewModel.LauncherViewModel.CurrentMod.Exists();
        }

        private void ModNotExistsTasks()
        {
            ModFoundIndicator = SetColor(IndicatorColor.Red);
            ModFoundMessage = GetMessage("CheckMessageModNotFound");
            ResetUi();
        }

        private void ModExistsTasks()
        {
            ModFoundIndicator = SetColor(IndicatorColor.Green);
            ModFoundMessage = GetMessage("CheckMessageModFound");
        }

        #endregion

        #region CheckPatch

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
            var a = LauncherPane.MainWindowViewModel.LauncherViewModel.Eaw.IsPatched();
            await AnimateProgressBar(Progress, 51, 10, this, x => x.Progress);
            var b = LauncherPane.MainWindowViewModel.LauncherViewModel.BaseGame.IsPatched();
            await AnimateProgressBar(Progress, 101, 10, this, x => x.Progress);
            return a && b;
        }

        private void GamesNotUpdated()
        {
            GamesPatchedIndicator = SetColor(IndicatorColor.Red);
            GamesPatchedMessage = GetMessage("CheckMessageGamesNotPatched");
            ResetUi();
            Show(GetMessage("CheckGamesNotPatchedMessage"));
        }

        private void GamesUpdated()
        {
            GamesPatchedIndicator = SetColor(IndicatorColor.Green);
            GamesPatchedMessage = GetMessage("CheckMessageGamesPatched");
        }

        #endregion

        #region CheckAI

        /// <summary>
        /// Checks the AiFileContainer
        /// </summary>
        /// <returns>returns if successfull or not</returns>
        private async Task<bool> CheckAiCorrectAsync()
        {
            if (!await CheckFolderListAsync(AiFolderList, new List<string> {@"\Data\CustomMaps\"}))
            {
                AiWrongInstalled();
                var result = Show(GetMessage("CheckAIFolderNotValid"), "Republic at War", MessageBoxButton.YesNo,
                    MessageBoxImage.Error, MessageBoxResult.Yes);
                if (result == MessageBoxResult.Yes)
                    _messageRecorder.Save(GetMessage("CheckFolderNotValid"));
                return false;
            }
            AiCorrectInstalled();
            return true;
        }

        private void AiCorrectInstalled()
        {
            ModAiIndicator = SetColor(IndicatorColor.Green);
            ModAiMessage = GetMessage("CheckMessageAiCorrect");
        }

        private void AiWrongInstalled()
        {
            ModAiIndicator = SetColor(IndicatorColor.Red);
            ModAiMessage = _mSource.Token.IsCancellationRequested ? GetMessage("CheckMessageAborted") : GetMessage("CheckMessageAiWrong");
            ResetUi();
        }

        #endregion

        #region CheckMod

        /// <summary>
        /// Checks the AiFileContainer
        /// </summary>
        /// <returns>returns if successfull or not</returns>
        private async Task<bool> CheckModCorrectAsync()
        {
            var excludeList = new List<string> {@"\Data\Audio\Speech\*", @"\", @"\Data\", @"\Data\Text\", @"\Data\Audio\"};
            if (!await CheckFolderListAsync(ModFolderList, excludeList))
            {
                ModWrongInstalled();
                var result = Show(GetMessage("CheckModFolderNotValid"), "Republic at War", MessageBoxButton.YesNo,
                    MessageBoxImage.Error, MessageBoxResult.Yes);
                if (result == MessageBoxResult.Yes)
                    _messageRecorder.Save(GetMessage("CheckFolderNotValid"));
                return false;
            }
            ModCorrectInstalled();
            return true;
        }

        private void ModCorrectInstalled()
        {
            ModFilesIndicator = SetColor(IndicatorColor.Green);
            ModFilesMessage = GetMessage("CheckMessageModCorrect");
        }

        private void ModWrongInstalled()
        {
            ModFilesIndicator = SetColor(IndicatorColor.Red);
            ModFilesMessage = _mSource.Token.IsCancellationRequested ? GetMessage("CheckMessageAborted") : GetMessage("CheckMessageModWrong");
            ResetUi();
        }

        #endregion

        #region PrepareXml

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
            if (!HostServer.IsRunning())
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
            var validator = new XmlValidator(Resources.FileContainer.ToStream());

            if (!validator.Validate(CheckFileStream))
            {
                ModCheckError(GetMessage("CheckXmlValidateError"));
                return false;
            }
            return true;
        }

        /// <summary>
        /// Writes XML-Stream to local file for offline check
        /// </summary>
        private void WriteOnlineDataToDisk()
        {
            HostServer.DownloadString(Config.ModVersionListRelativePath)
                .ToStream()
                .ToFile(LauncherViewModel.RestoreDownloadDir + Config.ModVersionListRelativePath);
            if (CheckFileStream.IsEmpty())
                return;
            CheckFileStream.ToFile(LauncherViewModel.GetRescueFilePath(CheckFileFileName, false));
        }

        /// <summary>
        /// Parses the XML-Stream, checks the Version of it and fills the Containers
        /// </summary>
        /// <returns></returns>
        private bool ParseXmlFile()
        {
            var parser = new XmlObjectParser<FileContainer>(CheckFileStream);
            FileContainer = parser.Parse();

            if (FileContainer.Version != LauncherPane.MainWindowViewModel.LauncherViewModel.CurrentMod.Version)
            {
                ModCheckError(GetMessage("CheckXmlWrongVersion"));
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
            if (!VersionUtilities.GetAllAvailableModVersionsOffline().Contains(LauncherViewModel.CurrentMod.Version))
            {
                Show(GetMessage("CheckVersionNotFound"));
                return;
            }
            if (!File.Exists(LauncherViewModel.GetRescueFilePath(CheckFileFileName, false)))
            {
                ModCheckError(GetMessage("CheckOfflineXmlNotFound"));
                return;
            }
            CheckFileStream = FileToStream(LauncherViewModel.GetRescueFilePath(CheckFileFileName, false));
        }

        /// <summary>
        /// Tries to get online Restore XML
        /// </summary>
        private async Task GetOnlineAsync()
        {
            if (!VersionUtilities.GetAllAvailableModVersionsOnline().Contains(LauncherViewModel.CurrentMod.Version))
            {
                Show(GetMessage("CheckVersionNotFound"));
                return;
            }
            await
                Task.Factory.StartNew(
                    () =>
                        CheckFileStream =
                            HostServer.DownloadString(LauncherViewModel.GetRescueFilePath(CheckFileFileName, true))
                                .ToStream());
        }

        private void ModCheckError(string message)
        {
            ModAiIndicator = SetColor(IndicatorColor.Red);
            ModAiMessage = GetMessage("CheckMessageCouldNotCheck");
            ModFilesIndicator = SetColor(IndicatorColor.Red);
            ModFilesMessage = GetMessage("CheckMessageCouldNotCheck");
            ResetUi();
            if (!IsNullOrEmpty(message))
                Show(message);
        }

        #endregion

        #region Commands

        /// <summary>
        /// Patch the Games
        /// </summary>
        public Command PatchGamesCommand => new Command(PatchGames);

        private void PatchGames()
        {
            AudioHelper.PlayAudio(AudioHelper.Audio.ButtonPress);
            var eaw = LauncherPane.MainWindowViewModel.LauncherViewModel.Eaw.Patch();
            var foc = LauncherPane.MainWindowViewModel.LauncherViewModel.BaseGame.Patch();
            CreatePatchMessage(eaw, foc);
            PerformCheckAsync();
        }

        /// <summary>
        /// Check the installed Mod
        /// </summary>
        public Command CheckVersionCommand => new Command(CheckVersion);

        private void CheckVersion()
        {
            AudioHelper.PlayAudio(AudioHelper.Audio.ButtonPress); 
            PerformCheckAsync();
        }

        /// <summary>
        /// Cancel the Operation
        /// </summary>
        public Command CancelCommand => new Command(Cancel);

        private void Cancel()
        {
            _mSource?.Cancel(false);
        }

        #endregion
    }
}