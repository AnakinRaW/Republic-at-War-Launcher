using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ModernApplicationFramework.Input.Base;
using ModernApplicationFramework.Input.Command;
using RawLauncherWPF.ExtensionClasses;
using RawLauncherWPF.Hash;
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
    public abstract class RestoreUpdateViewModel : LauncherPaneViewModel, IHasProgressBar, ICancelable, IHasSelection
    {
        protected const string RestoreUpdateContainerFileName = "RestoreModFileContainer.xml";

        private double _progress;
        private string _progressStatus;

        protected CancellationTokenSource MSource;

        protected RestoreUpdateViewModel(ILauncherPane pane) : base(pane)
        {
            LauncherViewModel = LauncherPane.MainWindowViewModel.LauncherViewModel;
        }

        protected abstract RestoreUpdateOperation ViewModelOperation { get; }

        protected static IHostServer HostServer => LauncherViewModel.HostServerStatic;

        public LauncherViewModel LauncherViewModel { get; }

        /// <summary>
        ///     Data which contains the informations to update/Restore the Mod
        /// </summary>
        protected RestoreTable RestoreTable { get; set; }

        /// <summary>
        ///     Container with all information extracted from Restore XML File/Stream
        /// </summary>
        protected FileContainer FileContainer { get; set; }

        /// <summary>
        ///     Stream which contains the XML data of the version to restore
        /// </summary>
        protected Stream XmlFileStream { get; set; }

        public ICommand CancelCommand => new Command(Cancel);

        public void Cancel()
        {
            MSource?.Cancel(false);
        }

        public double Progress
        {
            get => _progress;
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
            get => _progressStatus;
            set
            {
                if (Equals(value, _progressStatus))
                    return;
                _progressStatus = value;
                OnPropertyChanged();
            }
        }

        public Command<object> ChangeSelectionCommand => new ObjectCommand(ChangeSelection, CanChangeSelection);

        protected virtual void ChangeSelection(object obj)
        {
            AudioHelper.PlayAudio(AudioHelper.Audio.ButtonPress);
        }

        protected virtual bool CanChangeSelection(object arg)
        {
            return true;
        }


        /// <summary>
        ///     Procedure to Parse the XMLFile into an Object
        /// </summary>
        protected void ParseXmlToFileContainer()
        {
            var parser = new XmlObjectParser<FileContainer>(XmlFileStream);
            FileContainer = parser.Parse();
        }

        /// <summary>
        ///     Inits the ProgressBar and Blocks other commands
        /// </summary>
        protected void PrepareUi()
        {
            Progress = 0;
            IsBlocking = true;
            IsWorking = true;
        }

        protected virtual void ResetUi()
        {
            IsWorking = false;
            IsBlocking = false;
            XmlFileStream = Stream.Null;
            FileContainer = null;
            RestoreTable = null;
        }

        /// <summary>
        ///     Fills restore Table with files that need to be deleted
        /// </summary>
        /// <param name="shallIgnore"></param>
        /// <returns></returns>
        protected async Task<UpdateRestoreStatus> AddDeleteFilesToRestoreTable(bool shallIgnore)
        {
            var i = (double) 100 / FileContainer.Files.Count;
            await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);

            try
            {
                ProzessStatus = GetMessage(ViewModelOperation == RestoreUpdateOperation.Restore
                    ? "RestoreStatusCheckAdditionalFiles"
                    : "UpdateStatusCheckAdditionalFiles");


                //Find unused files to delete (AI Files)
                if (Directory.Exists(LauncherViewModel.BaseGame.GameDirectory + "\\Data\\"))
                {
                    var files = Directory.EnumerateFiles(LauncherViewModel.BaseGame.GameDirectory + "\\Data\\XML\\",
                        "*.*", SearchOption.AllDirectories).ToList();
                    files.AddRange(Directory.EnumerateFiles(
                        LauncherViewModel.BaseGame.GameDirectory + "\\Data\\Scripts\\", "*.*",
                        SearchOption.AllDirectories));
                    files.AddRange(Directory.EnumerateFiles(
                        LauncherViewModel.BaseGame.GameDirectory + "\\Data\\CustomMaps\\", "*.*",
                        SearchOption.AllDirectories));

                    foreach (var file in files)
                    {
                        var flag =
                            await
                                Task.Run(
                                    () =>
                                        FileContainer.Files.Any(
                                            k =>
                                                k.Name.Equals(Path.GetFileName(file),
                                                    StringComparison.InvariantCultureIgnoreCase) &&
                                                k.TargetType == TargetType.Ai &&
                                                Path.GetFullPath(file)
                                                    .IndexOf(k.TargetPath,
                                                        StringComparison.InvariantCultureIgnoreCase) >= 0),
                                    MSource.Token);

                        Progress = Progress + i;
                        if (flag)
                            continue;
                        RestoreTable.Files.Add(RestoreFile.CreateDeleteFile(file, TargetType.Ai));
                    }
                }


                //Find unused files to delete (Mod Files)
                if (!Directory.Exists(LauncherViewModel.CurrentMod.ModDirectory))
                    return UpdateRestoreStatus.Succeeded;
                foreach (
                    var file in
                    await
                        Task.Run(
                            () =>
                                Directory.EnumerateFiles(LauncherViewModel.CurrentMod.ModDirectory, "*.*",
                                    SearchOption.AllDirectories), MSource.Token))
                {
                    if (ViewModelOperation == RestoreUpdateOperation.Update &&
                        new FileInfo(file).Directory?.Name == "Text")
                    {
                        Progress = Progress + i;
                        continue;
                    }


                    if (shallIgnore && RestoreUpdateUtilities.IgnoreFile(file, ViewModelOperation))
                    {
                        Progress = Progress + i;
                        continue;
                    }


                    var flag =
                        await
                            Task.Run(
                                () =>
                                    FileContainer.Files.Any(
                                        k =>
                                            k.Name == Path.GetFileName(file) && k.TargetType == TargetType.Mod &&
                                            Path.GetFullPath(file)
                                                .IndexOf(k.TargetPath, StringComparison.CurrentCultureIgnoreCase) >= 0),
                                MSource.Token);


                    Progress = Progress + i;
                    if (flag)
                        continue;
                    RestoreTable.Files.Add(RestoreFile.CreateDeleteFile(file, TargetType.Mod));
                }
            }
            catch (TaskCanceledException)
            {
                Show(ViewModelOperation == RestoreUpdateOperation.Restore
                    ? GetMessage("RestoreAborted")
                    : GetMessage("UpdateAborted"));
                return UpdateRestoreStatus.Canceled;
            }
            return UpdateRestoreStatus.Succeeded;
        }


        /// <summary>
        ///     Fille RestoreTable with files taht need to be downloaded
        /// </summary>
        /// <param name="version"></param>
        /// <param name="excludeList"></param>
        /// <returns></returns>
        protected async Task<UpdateRestoreStatus> AddDownloadFilesToRestoreTable(Version version, List<string> excludeList)
        {
            RestoreTable = new RestoreTable(version);
            if (version != FileContainer.Version)
                return UpdateRestoreStatus.Error;

            var hashProvider = new HashProvider();
            var listToCheck = FileContainer.Files;
            if (excludeList != null)
                listToCheck = FileContainerFile.ListFromExcludeList(FileContainer.Files, excludeList);

            var i = (double) 100 / listToCheck.Count;

            try
            {
                //Find missing/corrupted files to download
                ProzessStatus = GetMessage(ViewModelOperation == RestoreUpdateOperation.Restore
                    ? "RestoreStatusCheckMissing"
                    : "UpdateStatusCheckNew");

                var t = listToCheck.Select(file => Task.Run(async () =>
                {
                    var absolutePath = CreateAbsoluteFilePath(file);
                    if (!await Task.Run(() => File.Exists(absolutePath), MSource.Token) ||
                        await Task.Run(() => hashProvider.GetFileHash(absolutePath) != file.Hash, MSource.Token))
                    {
                        var restoreFile = RestoreFile.CreateResotreFile(file, FileAction.Download);
                        RestoreTable.Files.Add(restoreFile);
                    }
                    Progress = Progress + i;
                }));
                await Task.WhenAll(t.ToArray());
            }
            catch (TaskCanceledException)
            {
                return UpdateRestoreStatus.Canceled;
            }
            //finally
            //{
            //    if (HostServer.HasErrors)
            //        HostServer.ShowLog();
            //}
            return UpdateRestoreStatus.Succeeded;
        }

        /// <summary>
        ///     Downloads the marked files in RestoreTable
        /// </summary>
        /// <returns></returns>
        protected async Task<UpdateRestoreStatus> DownloadFiles()
        {
            var filesToDownload = RestoreTable.GetFilesOfAction(FileAction.Download);
            var i = (double) 100 / filesToDownload.Count;
            try
            {
                var t = filesToDownload.Select(file => Task.Run(async () =>
                {
                    if (!ComputerHasInternetConnection())
                    {
                        Show(ViewModelOperation == RestoreUpdateOperation.Restore
                            ? GetMessage("RestoreInternetLost")
                            : GetMessage("UpdateInternetLost"));
                        return;
                    }
                    var restorePath = CreateAbsoluteFilePath(file);
                    await
                        Task.Run(() => HostServer.DownloadFile(file.SourcePath, restorePath),
                            MSource.Token);
                    ProzessStatus =
                        GetMessage(
                            ViewModelOperation == RestoreUpdateOperation.Restore
                                ? "RestoreStatusDownloaded"
                                : "UpdateStatusDownloaded", file.Name);
                    Progress = Progress + i;
                }));

                await Task.WhenAll(t.ToArray());
            }
            catch (TaskCanceledException)
            {
                return UpdateRestoreStatus.Canceled;
            }
            //finally
            //{
            //    if (HostServer.HasErrors)
            //        HostServer.ShowLog();
            //}
            return UpdateRestoreStatus.Succeeded;
        }

        /// <summary>
        ///     Deletes all files marked in the RestoreTable
        /// </summary>
        protected async Task DeleteUnneededFiles()
        {
            foreach (var file in await Task.Run(() => RestoreTable.GetFilesOfAction(FileAction.Delete)))
            {
                var deletePath = CreateLocalFilePath(file);
                File.Delete(deletePath);
            }
        }

        /// <summary>
        ///     Main Procedure to Download and Delete marked files
        /// </summary>
        /// <returns></returns>
        protected async Task<UpdateRestoreStatus> InternalRestoreUpdate()
        {
            if (RestoreTable == null)
                return UpdateRestoreStatus.Error;
            if (RestoreTable.Files.Count == 0)
                return UpdateRestoreStatus.Succeeded;
            if (await DownloadFiles() == UpdateRestoreStatus.Canceled)
                return UpdateRestoreStatus.Canceled;
            await DeleteUnneededFiles();
            return UpdateRestoreStatus.Succeeded;
        }


        /// <summary>
        ///     Procedure to Download and Validate the XML File
        /// </summary>
        /// <returns>False if failed</returns>
        protected async Task<LoadRestoreUpdateResult> LoadXmlFileStream(Version version)
        {
            if (!HostServer.IsRunning())
                return LoadRestoreUpdateResult.Offline;

            if (!GetAllAvailableModVersionsOnline().Contains(version))
                return LoadRestoreUpdateResult.WrongVersion;

            var downloadPath = LauncherViewModel.GetRescueFilePath(RestoreUpdateContainerFileName, true,
                version);

            await
                Task.Factory.StartNew(
                    () => XmlFileStream = HostServer.DownloadString(downloadPath).ToStream());

            if (XmlFileStream.IsEmpty())
                return LoadRestoreUpdateResult.StreamEmpty;

            var validator = new XmlValidator(Resources.FileContainer.ToStream());
            if (!validator.Validate(XmlFileStream))
                return LoadRestoreUpdateResult.StreamBroken;
            return LoadRestoreUpdateResult.Suceeded;
        }

        /// <summary>
        ///     Main Prcedure to get the RestoreXML Data
        /// </summary>
        /// <returns>False if failed</returns>
        protected async Task<LoadRestoreUpdateResult> GetXmlData(Version version)
        {
            var result = await LoadXmlFileStream(version);
            if (result != LoadRestoreUpdateResult.Suceeded)
                return result;
            await AnimateProgressBar(Progress, 50, 1, this, x => x.Progress);
            await Task.Run(() => ParseXmlToFileContainer());
            await AnimateProgressBar(Progress, 100, 1, this, x => x.Progress);
            return LoadRestoreUpdateResult.Suceeded;
        }


        protected PrepareUpdateRestoreResult PrepareUpdateRestore(Version version)
        {
            HostServer.FlushErrorLog();
            if (!ComputerHasInternetConnection())
                return PrepareUpdateRestoreResult.NoInternet;
            if (version == null)
                return PrepareUpdateRestoreResult.NoVersion;
            return !AskUserToContinue() ? PrepareUpdateRestoreResult.Canceled : PrepareUpdateRestoreResult.Succeeded;
        }

        protected abstract bool AskUserToContinue();

        private string CreateLocalFilePath(RestoreFile file)
        {
            if (file.TargetType == TargetType.Ai)
                return Path.Combine(LauncherViewModel.BaseGame.GameDirectory, file.TargetPath);
            return Path.Combine(LauncherViewModel.CurrentMod.ModDirectory, file.TargetPath);
        }

        private string CreateAbsoluteFilePath(FileContainerFile file)
        {
            if (file.TargetType == TargetType.Ai)
                return Path.GetFullPath(LauncherViewModel.BaseGame.GameDirectory + file.TargetPath);
            return Path.GetFullPath(LauncherViewModel.CurrentMod.ModDirectory + file.TargetPath);
        }

        private string CreateAbsoluteFilePath(RestoreFile file)
        {
            if (file.TargetType == TargetType.Ai)
                return LauncherViewModel.BaseGame.GameDirectory + file.TargetPath;
            return Path.Combine(LauncherViewModel.CurrentMod.ModDirectory, file.TargetPath);
        }
    }

    public interface IHasSelection
    {
        Command<object> ChangeSelectionCommand { get; }
    }

    public interface ICancelable
    {
        ICommand CancelCommand { get; }

        void Cancel();
    }

    public interface IHasProgressBar
    {
        double Progress { get; set; }

        string ProzessStatus { get; set; }
    }

    public enum UpdateRestoreStatus
    {
        Canceled,
        Error,
        Succeeded
    }

    public enum PrepareUpdateRestoreResult
    {
        Canceled,
        NoInternet,
        NoVersion,
        Succeeded
    }

    public enum LoadRestoreUpdateResult
    {
        Offline,
        WrongVersion,
        StreamEmpty,
        StreamBroken,
        Suceeded
    }
}