using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ModernApplicationFramework.Commands;
using RawLauncherWPF.ExtensionClasses;
using RawLauncherWPF.Helpers;
using RawLauncherWPF.Models;
using RawLauncherWPF.Properties;
using RawLauncherWPF.Server;
using RawLauncherWPF.UI;
using RawLauncherWPF.Utilities;
using RawLauncherWPF.Xml;
using static RawLauncherWPF.NativeMethods.NativeMethods;
using static RawLauncherWPF.Utilities.MessageProvider;
using static RawLauncherWPF.Utilities.ProgressBarUtilities;
using static RawLauncherWPF.Utilities.VersionUtilities;

namespace RawLauncherWPF.ViewModels
{
    public sealed class UpdateViewModel : LauncherPaneViewModel
    {
        private const string ChangelogFileName = "Changelog.txt";
        private const string UpdateFileName = "RestoreModFileContainer.xml";
        private CancellationTokenSource _mSource;
        private double _progress;
        private string _progressStatus;

        public UpdateViewModel(ILauncherPane pane) : base(pane)
        {
            LauncherViewModel = LauncherPane.MainWindowViewModel.LauncherViewModel;
        }

        /// <summary>
        /// Progress from 0 to 100
        /// </summary>
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

        /// <summary>
        /// Indicating message what the Launcehr is doing
        /// </summary>
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

        /// <summary>
        /// Reference to the HostServer
        /// </summary>
        private IHostServer HostServer => LauncherViewModel.HostServerStatic;

        /// <summary>
        /// Reference to the LauncherViewModel
        /// </summary>
        private LauncherViewModel LauncherViewModel { get; }

        /// <summary>
        /// Selected Restore Option
        /// </summary>
        private UpdateOptions SelectedOption { get; set; }

        /// <summary>
        /// Container with all information extracted from Restore XML File/Stream
        /// </summary>
        private FileContainer UpdateContainer { get; set; }

        /// <summary>
        /// Stream which contains the XML data of the version to restore
        /// </summary>
        private Stream UpdateFileStream { get; set; }

        /// <summary>
        /// Data which contains the informations to update the Mod
        /// </summary>
        private RestoreTable UpdateTable { get; set; }

        public async void PerformUpdate()
        {
            var l = LauncherViewModel.CurrentMod.InstalledLanguage;
            if (!ComputerHasInternetConnection())
            {
                Show("You need an Internet connction to Restore your mod");
                return;
            }
            if (!UpdateHelper.AskUserToContinue())
                return;
            PrepareUi();
            ProzessStatus = "Preparing Update";
            await AnimateProgressBar(Progress, 10, 0, this, x => x.Progress);
            if (!await GetUpdateXmlData())
            {
                ResetUi();
                return;
            }

            await ThreadUtilities.SleepThread(250);
            await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);

            switch (SelectedOption)
            {
                case UpdateOptions.None:
                case 0:
                    if (!await PrepareNormalUpdate())
                    {
                        ResetUi();
                        return;
                    }
                    break;
                default:
                    if (!await PrepareVoiceIgnoreUpdate())
                    {
                        ResetUi();
                        return;
                    }
                    break;
            }
            await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);

            if (!await InternalUpdate())
            {
                Show("Either you aborted the Progress of something failed");
                ResetUi();
                return;
            }
            LauncherPane.MainWindowViewModel.InstalledVersion = LauncherViewModel.CurrentMod.Version;
            await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);
            ProzessStatus = "Finishing";
            await Task.Run(() =>
            {
                var model =  LauncherPane.MainWindowViewModel.LauncherPanes[2].ViewModel;
                var languageModel = (LanguageViewModel) model;
                languageModel?.ChangeLanguage(l);
            });
            await AnimateProgressBar(Progress, 101, 10, this, x => x.Progress);
            Show("Updateing Done");
            ResetUi();
        }

        private async Task<bool> AddDeleteFilesToUpdateTable(bool shallIgnore)
        {
            var i = (double)100 / UpdateContainer.Files.Count;
            await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);
            try
            {
                ProzessStatus = "Checking for additional files";

                //Find files to delete (AI files)
                if (Directory.Exists(LauncherViewModel.BaseGame.GameDirectory + "\\Data\\"))
                {
                    foreach (var file in await Task.Run(() => Directory.EnumerateFiles(LauncherViewModel.BaseGame.GameDirectory + "\\Data\\", "*.*", SearchOption.AllDirectories), _mSource.Token))
                    {
                        var fileToSearch = await Task.Run(
                            () =>
                                UpdateContainer.Files.Find(
                                    k =>
                                        k.Name == Path.GetFileName(file) && k.TargetType == TargetType.Ai &&
                                        Path.GetFullPath(file).Contains(k.TargetPath)), _mSource.Token);
                        Progress = Progress + i;
                        if (fileToSearch != null)
                            continue;
                        // File on disk was not found in XML 
                        UpdateTable.Files.Add(RestoreFile.CreateDeleteFile(file, TargetType.Ai));
                    }
                }

                //Find files to delete (Mod files)
                foreach (var file in await Task.Run(() => Directory.EnumerateFiles(LauncherViewModel.CurrentMod.ModDirectory, "*.*", SearchOption.AllDirectories), _mSource.Token))
                {
                    if (new FileInfo(file).Directory?.Name == "Text")
                    {
                        Progress = Progress + i;
                        continue;
                    }
                    if (shallIgnore && UpdateHelper.IgnoreFile(file))
                    {
                        Progress = Progress + i;
                        continue;
                    }
                    var fileToSearch = await Task.Run(
                        () =>
                            UpdateContainer.Files.Find(
                                k =>
                                    k.Name == Path.GetFileName(file) && k.TargetType == TargetType.Mod &&
                                    Path.GetFullPath(file).Contains(k.TargetPath)), _mSource.Token);
                    Progress = Progress + i;
                    if (fileToSearch == null)
                        UpdateTable.Files.Add(RestoreFile.CreateDeleteFile(file, TargetType.Mod));
                }
            }
            catch (TaskCanceledException)
            {
                Show("Restoring aborted");
                return false;
            }
            return true;
        }

        private async Task<bool> AddDownloadFilesToUpdateTable(List<string> excludeList)
        {
            var versionToUpdate = GetLatestVersion();
            UpdateTable = new RestoreTable(versionToUpdate);
            if (versionToUpdate != UpdateContainer.Version)
                throw new Exception("Versions do not match");

            var hashProvider = new Hash.HashProvider();
            var listToCheck = UpdateContainer.Files;
            if (excludeList != null)
                listToCheck = FileContainerFile.ListFromExcludeList(UpdateContainer.Files, excludeList);

            var i = (double) 100/listToCheck.Count;

            try
            {
                // Find missing/corrupted files to download
                ProzessStatus = "Checkign for new and corruppted files";
                foreach (var file in listToCheck)
                {
                    var absolutePath = CreateAbsoluteFilePath(file);

                    //If file does not exists and is corrupted
                    if (!await Task.Run(() => File.Exists(absolutePath), _mSource.Token) ||
                        await
                            Task.Run(
                                () =>
                                    (hashProvider.GetFileHash(absolutePath) != file.Hash &&
                                     !FileContainerFile.ShallExclude(file, excludeList)), _mSource.Token))
                    {
                        var restoreFile = RestoreFile.CreateResotreFile(file, FileAction.Download);
                        UpdateTable.Files.Add(restoreFile);

                    }
                    Progress = Progress + i;
                }

            }
            catch (TaskCanceledException)
            {
                Show("Update aborted");
                return false;
            }
            return true;
        }

        private string CreateAbsoluteFilePath(RestoreFile file)
        {
            if (file.TargetType == TargetType.Ai)
                return LauncherViewModel.BaseGame.GameDirectory + file.TargetPath;
            return Path.Combine(LauncherViewModel.CurrentMod.ModDirectory, file.TargetPath);
        }

        private string CreateAbsoluteFilePath(FileContainerFile file)
        {
            if (file.TargetType == TargetType.Ai)
                return LauncherViewModel.BaseGame.GameDirectory + file.TargetPath;
            return LauncherViewModel.CurrentMod.ModDirectory + file.TargetPath;
        }

        private string CreateLocalFilePath(RestoreFile file)
        {
            if (file.TargetType == TargetType.Ai)
                return Path.Combine(LauncherViewModel.BaseGame.GameDirectory, file.TargetPath);
            return Path.Combine(LauncherViewModel.CurrentMod.ModDirectory, file.TargetPath);
        }

        private async Task DeleteUnneededFiles()
        {
            foreach (var file in await Task.Run(() => UpdateTable.GetFilesOfAction(FileAction.Delete)))
            {
                var deletePath = CreateLocalFilePath(file);
                File.Delete(deletePath);
            }
        }

        private async Task<bool> DownloadUpdateFiles()
        {
            var filesToDownload = UpdateTable.GetFilesOfAction(FileAction.Download);
            var i = (double) 100/filesToDownload.Count;
            try
            {
                var t = filesToDownload.Select(file => Task.Run(async () =>
                {
                    if (!ComputerHasInternetConnection())
                    {
                        Show(
                            "You lost your Internet connection. In order to prevent much more error messages the progress will be cancelled now.");
                        return;
                    }
                    var updatePath = CreateAbsoluteFilePath(file);
                    await
                        Task.Run(() => HostServer.DownloadFile("Versions" + file.SourcePath, updatePath),
                            _mSource.Token);
                    ProzessStatus = "Downloaded: " + file.Name;
                    Progress = Progress + i;
                }));

                await Task.WhenAll(t.ToArray());
            }
            catch (TaskCanceledException)
            {
                Show("Restoring aborted");
                return false;
            }
            return true;
        }

        private async Task<bool> InternalUpdate()
        {
            if (UpdateTable == null)
            {
                Show("Error while trying to download the mod files. The required Table was empty \r\nPlease try again");
                return false;
            }
            if (!await DownloadUpdateFiles())
                return false;
            await DeleteUnneededFiles();
            return true;
        }

        /// <summary>
        /// Inits the ProgressBar and Blocks other commands
        /// </summary>
        private void PrepareUi()
        {
            Progress = 0;
            IsBlocking = true;
            IsWorking = true;
        }

        /// <summary>
        /// Restets UI to initail state
        /// </summary>
        private void ResetUi()
        {
            IsWorking = false;
            IsBlocking = false;
            UpdateFileStream = Stream.Null;
            UpdateContainer = null;
            UpdateTable = null;
        }

        #region Normal

        private async Task<bool> PrepareNormalUpdate()
        {
            await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);
            ProzessStatus = "Preparing download-table";
            if (!await FillUpdateTableNormal())
                return false;
            return true;
        }

        private async Task<bool> FillUpdateTableNormal()
        {
            if (!await AddDownloadFilesToUpdateTable(null))
                return false;
            if (!await AddDeleteFilesToUpdateTable(false))
                return false;
            return true;
        }

        #endregion

        #region IgnoreVoice

        private async Task<bool> PrepareVoiceIgnoreUpdate()
        {
            await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);
            ProzessStatus = "Preparing download-table";
            if (!await FillUpdateTableVoice())
                return false;
            return true;
        }

        private async Task<bool> FillUpdateTableVoice()
        {
            if (!await AddDownloadFilesToUpdateTable(new List<string>
                    {
                        @"\Data\Audio\Speech\*",
                        @"\Data\*Speech.meg",
                        @"\Data\Audio\"
                    }))
                return false;
            if (!await AddDeleteFilesToUpdateTable(false))
                return false;
            return true;
        }

        #endregion

        #region UpdateXML

        /// <summary>
        /// Main Prcedure to get the Update XML Data
        /// </summary>
        /// <returns>False if failed</returns>
        private async Task<bool> GetUpdateXmlData()
        {
            if (!await LoadUpdateStream())
                return false;
            await AnimateProgressBar(Progress, 50, 1, this, x => x.Progress);
            if (!await Task.FromResult(ParseUpdateXml()))
                return false;
            await AnimateProgressBar(Progress, 100, 1, this, x => x.Progress);
            return true;
        }

        /// <summary>
        /// Procedure to Download and Validate the XML File
        /// </summary>
        /// <returns>False if failed</returns>
        private async Task<bool> LoadUpdateStream()
        {
            if (!HostServer.IsRunning())
            {
                Show("Could not Download the required files, because the servers are offline.\r\n" +
                     "Please try later");
                return false;
            }
            if (!GetAllAvailableVersionsOnline().Contains(GetLatestVersion()))
            {
                Show("Your installed version is not available to check. Please try later or contact us.");
                return false;
            }

            var downloadPath = LauncherViewModel.GetRescueFilePath(UpdateFileName, true, GetLatestVersion());
            await
                Task.Factory.StartNew(
                    () => UpdateFileStream = HostServer.DownloadString(downloadPath).ToStream());

            if (UpdateFileStream.IsEmpty())
            {
                Show("Error while downloading the required files.\r\n" + "Please try later");
                return false;
            }

            var validator = new XmlValidator(Resources.FileContainer.ToStream());
            if (!validator.Validate(UpdateFileStream))
            {
                Show(
                    "The necessary files are not valid. It was also not possible to check them with our server. Please click Restore-Tab and let the launcher redownload the Files.");
                return false;
            }
            return true;

        }

        /// <summary>
        /// Procedure to Parse the XMLFile into an Object
        /// </summary>
        /// <returns>False if failed</returns>
        private bool ParseUpdateXml()
        {
            var parser = new XmlObjectParser<FileContainer>(UpdateFileStream);
            UpdateContainer = parser.Parse();
            return true;
        }

        #endregion

        #region Commands

        public Command UpdateModCommand => new Command(UpdateMod);

        private void UpdateMod()
        {
            AudioHelper.PlayAudio(AudioHelper.Audio.ButtonPress);
            _mSource = new CancellationTokenSource();
            PerformUpdate();
        }

        public Command<object> ChangeSelectionCommand => new Command<object>(ChangeSelection, CanChangeSelection);

        private void ChangeSelection(object obj)
        {
            AudioHelper.PlayAudio(AudioHelper.Audio.ButtonPress);
            SelectedOption = (UpdateOptions)obj;
        }

        private bool CanChangeSelection(object arg)
        {
            return true;
        }

        public Command OpenChangelogCommand => new Command(OpenChangelog);

        private async void OpenChangelog()
        {
            AudioHelper.PlayAudio(AudioHelper.Audio.ButtonPress);
            await Task.Run(() => HostServer.DownloadFile("Versions/" + ChangelogFileName, LauncherViewModel.RestoreDownloadDir + ChangelogFileName));
            var process = new Process { StartInfo = { FileName = LauncherViewModel.RestoreDownloadDir + ChangelogFileName } };
            process.Start();
        }

        public Command CancelCommand => new Command(Cancel);

        private void Cancel()
        {
            _mSource?.Cancel(false);
        }

        #endregion
    }

    [Flags]
    public enum UpdateOptions
    {
        None = 1,
        IgnoreVoice = 2
    }
}
