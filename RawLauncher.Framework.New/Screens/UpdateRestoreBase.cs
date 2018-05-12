using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ModernApplicationFramework.Input.Command;
using RawLauncher.Framework.ExtensionClasses;
using RawLauncher.Framework.Hash;
using RawLauncher.Framework.Launcher;
using RawLauncher.Framework.Models;
using RawLauncher.Framework.Server;
using RawLauncher.Framework.Utilities;
using RawLauncher.Framework.Xml;

namespace RawLauncher.Framework.Screens
{
    public abstract class UpdateRestoreBase : LauncherScreen, IUpdateRestoreBase
    {
        protected const string RestoreUpdateContainerFileName = "RestoreModFileContainer.xml";

        private double _progress;
        private string _progressStatus;

        protected readonly LauncherModel Launcher;
        protected readonly IHostServer Server;
        protected CancellationTokenSource MSource;

        protected abstract RestoreUpdateOperation ViewModelOperation { get; }

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

        public ICommand ChangeSelectionCommand => new DelegateCommand(ChangeSelection);

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

        protected UpdateRestoreBase(LauncherModel launcher, IHostServer server)
        {
            Launcher = launcher;
            Server = server;
        }

        public void Cancel()
        {
            MSource?.Cancel(false);
        }

        protected virtual void ChangeSelection(object obj)
        {
            AudioPlayer.PlayAudio(AudioPlayer.Audio.ButtonPress);
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
            var i = (double)100 / FileContainer.Files.Count;
            await ProgressBarUtilities.AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);

            try
            {
                ProzessStatus = MessageProvider.GetMessage(ViewModelOperation == RestoreUpdateOperation.Restore
                    ? "RestoreStatusCheckAdditionalFiles"
                    : "UpdateStatusCheckAdditionalFiles");


                //Find unused files to delete (AI Files)
                if (Directory.Exists(Launcher.BaseGame.GameDirectory + "\\Data\\"))
                {
                    var files = new List<string>();
                    if (Directory.Exists(Path.Combine(Launcher.BaseGame.GameDirectory, @"\Data\XML\")))
                        files.AddRange(Directory.EnumerateFiles(Launcher.BaseGame.GameDirectory + "\\Data\\XML\\",
                            "*.*", SearchOption.AllDirectories).ToList());
                    if (Directory.Exists(Path.Combine(Launcher.BaseGame.GameDirectory, @"\Data\Scripts\")))
                        files.AddRange(Directory.EnumerateFiles(
                            Launcher.BaseGame.GameDirectory + "\\Data\\Scripts\\", "*.*",
                        SearchOption.AllDirectories));
                    if (Directory.Exists(Path.Combine(Launcher.BaseGame.GameDirectory, @"\Data\CustomMaps\")))
                        files.AddRange(Directory.EnumerateFiles(
                            Launcher.BaseGame.GameDirectory + "\\Data\\CustomMaps\\", "*.*",
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
                if (!Directory.Exists(Launcher.CurrentMod.ModDirectory))
                    return UpdateRestoreStatus.Succeeded;
                foreach (
                    var file in
                    await
                        Task.Run(
                            () =>
                                Directory.EnumerateFiles(Launcher.CurrentMod.ModDirectory, "*.*",
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
                MessageProvider.Show(ViewModelOperation == RestoreUpdateOperation.Restore
                    ? MessageProvider.GetMessage("RestoreAborted")
                    : MessageProvider.GetMessage("UpdateAborted"));
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

            var i = (double)100 / listToCheck.Count;

            try
            {
                //Find missing/corrupted files to download
                ProzessStatus = MessageProvider.GetMessage(ViewModelOperation == RestoreUpdateOperation.Restore
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
            var i = (double)100 / filesToDownload.Count;
            try
            {
                var t = filesToDownload.Select(file => Task.Run(async () =>
                {
                    if (!NativeMethods.NativeMethods.ComputerHasInternetConnection())
                    {
                        MessageProvider.Show(ViewModelOperation == RestoreUpdateOperation.Restore
                            ? MessageProvider.GetMessage("RestoreInternetLost")
                            : MessageProvider.GetMessage("UpdateInternetLost"));
                        return;
                    }
                    var restorePath = CreateAbsoluteFilePath(file);
                    await
                        Task.Run(() => Server.DownloadFile(file.SourcePath, restorePath),
                            MSource.Token);
                    ProzessStatus =
                        MessageProvider.GetMessage(
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
            if (!Server.IsRunning())
                return LoadRestoreUpdateResult.Offline;

            if (!VersionUtilities.GetAllAvailableModVersionsOnline().Contains(version))
                return LoadRestoreUpdateResult.WrongVersion;

            var downloadPath = Launcher.GetRescueFilePath(RestoreUpdateContainerFileName, true,
                version);

            await
                Task.Factory.StartNew(
                    () => XmlFileStream = Server.DownloadString(downloadPath).ToStream());

            if (XmlFileStream.IsEmpty())
                return LoadRestoreUpdateResult.StreamEmpty;

            var validator = new XmlValidator(Properties.Resources.FileContainer.ToStream());
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
            await ProgressBarUtilities.AnimateProgressBar(Progress, 50, 1, this, x => x.Progress);
            await Task.Run(() => ParseXmlToFileContainer());
            await ProgressBarUtilities.AnimateProgressBar(Progress, 100, 1, this, x => x.Progress);
            return LoadRestoreUpdateResult.Suceeded;
        }


        protected PrepareUpdateRestoreResult PrepareUpdateRestore(Version version)
        {
            Server.FlushErrorLog();
            if (!NativeMethods.NativeMethods.ComputerHasInternetConnection())
                return PrepareUpdateRestoreResult.NoInternet;
            if (version == null)
                return PrepareUpdateRestoreResult.NoVersion;
            return !AskUserToContinue() ? PrepareUpdateRestoreResult.Canceled : PrepareUpdateRestoreResult.Succeeded;
        }

        protected abstract bool AskUserToContinue();

        private string CreateLocalFilePath(RestoreFile file)
        {
            if (file.TargetType == TargetType.Ai)
                return Path.Combine(Launcher.BaseGame.GameDirectory, file.TargetPath);
            return Path.Combine(Launcher.CurrentMod.ModDirectory, file.TargetPath);
        }

        private string CreateAbsoluteFilePath(FileContainerFile file)
        {
            if (file.TargetType == TargetType.Ai)
                return Path.GetFullPath(Launcher.BaseGame.GameDirectory + file.TargetPath);
            return Path.GetFullPath(Launcher.CurrentMod.ModDirectory + file.TargetPath);
        }

        private string CreateAbsoluteFilePath(RestoreFile file)
        {
            if (file.TargetType == TargetType.Ai)
                return Launcher.BaseGame.GameDirectory + file.TargetPath;
            return Path.Combine(Launcher.CurrentMod.ModDirectory, file.TargetPath);
        }
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
