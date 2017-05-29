using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Interfaces;
using RawLauncherWPF.ExtensionClasses;
using RawLauncherWPF.Hash;
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

namespace RawLauncherWPF.ViewModels
{
    public sealed class RestoreViewModel : LauncherPaneViewModel
    {
        private const string RestoreVersionFileFileName = "RestoreModFileContainer.xml";
        private CancellationTokenSource _mSource;
        private double _progress;
        private string _progressStatus;

        private ComboBoxDataSource _dataSource;

        public RestoreViewModel(ILauncherPane pane) : base(pane)
        {
            LauncherViewModel = LauncherPane.MainWindowViewModel.LauncherViewModel;
            if (!ComputerHasInternetConnection())
                return;
            AvailableVersions = RestoreHelper.CreateVersionItems();
            DataSource = new ComboBoxDataSource(AvailableVersions);
        }

        /// <summary>
        /// Contains all ComboBoxItems with Versions
        /// </summary>
        public ObservableCollection<IHasTextProperty> AvailableVersions { get; set; }


        public ComboBoxDataSource DataSource
        {
            get => _dataSource;
            set
            {
                if (Equals(value, _dataSource)) return;
                _dataSource = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Progress from 0 to 100
        /// </summary>
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

        /// <summary>
        /// Indicating message what the Launcehr is doing
        /// </summary>
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

        /// <summary>
        /// Reference to the HostServer
        /// </summary>
        private IHostServer HostServer => LauncherViewModel.HostServerStatic;

        /// <summary>
        /// Reference to the LauncherViewModel
        /// </summary>
        private LauncherViewModel LauncherViewModel { get; }

        /// <summary>
        /// Data which contains the informations to restore the Mod
        /// </summary>
        private RestoreTable RestoreTable { get; set; }

        /// <summary>
        /// Container with all information extracted from Restore XML File/Stream
        /// </summary>
        private FileContainer RestoreVersionContainer { get; set; }

        /// <summary>
        /// Stream which contains the XML data of the version to restore
        /// </summary>
        private Stream RestoreVersionFileStream { get; set; }

        /// <summary>
        /// Selected Restore Option
        /// </summary>
        private RestoreOptions SelectedOption { get; set; }

        /// <summary>
        /// Main Procedure to Restore
        /// </summary>
        public async void PerformRestore()
        {
            var l = LauncherViewModel.CurrentMod.InstalledLanguage;
            if (!ComputerHasInternetConnection())
            {
                Show(GetMessage("RestoreNoInternet"));
                return;
            }
            if (DataSource.DisplayedItem == null)
            {
                Show(GetMessage("RestoreNoVersion"));
                return;
            }
            if (!RestoreHelper.AskUserToContinue())
                return;

            HostServer.FlushErrorLog();
            PrepareUi();
            ProzessStatus = GetMessage("RestoreStatusPrepare");
            await AnimateProgressBar(Progress, 10, 0, this, x => x.Progress);
            if (!await GetRestoreVersionXmlData())
            {
                ResetUi();
                return;
            }

            await ThreadUtilities.SleepThread(250);
            await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);

            switch (SelectedOption)
            {
                case RestoreOptions.None:
                case 0:
                    if (!await PrepareNormalRestore())
                    {
                        ResetUi();
                        return;
                    }
                    break;
                case RestoreOptions.Hard:
                    await PrepareHardRestore();
                    break;
                default:
                    if (!await PrepareLanguageIgnoreRestore())
                    {
                        ResetUi();
                        return;
                    }
                    break;
            }

            await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);

            if (!await InternalRestore())
            {
                ResetUi();
                return;
            }
            LauncherPane.MainWindowViewModel.InstalledVersion = LauncherViewModel.CurrentMod.Version;
            await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);
            ProzessStatus = GetMessage("RestoreStatusFinishing");
            await Task.Run(() =>
            {
                var model = LauncherPane.MainWindowViewModel.LauncherPanes[2].ViewModel;
                var languageModel = (LanguageViewModel)model;
                languageModel?.ChangeLanguage(l);
            });
            await AnimateProgressBar(Progress, 101, 10, this, x => x.Progress);
            Show(GetMessage("RestoreDone"));
            ResetUi();
        }

        /// <summary>
        /// Fills restore Table with files that need to be deleted
        /// </summary>
        /// <param name="shallIgnore"></param>
        /// <returns></returns>
        private async Task<bool> AddDeleteFilesToRestoreTable(bool shallIgnore)
        {
            var i = (double) 100/RestoreVersionContainer.Files.Count;
            await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);

            try
            {
                ProzessStatus = GetMessage("RestoreStatusCheckAdditionalFiles");


                //Find unused files to delete (AI Files)
                if (Directory.Exists(LauncherViewModel.BaseGame.GameDirectory + "\\Data\\"))
                {
                    var files = Directory.EnumerateFiles(LauncherViewModel.BaseGame.GameDirectory + "\\Data\\XML\\", "*.*", SearchOption.AllDirectories).ToList();
                    files.AddRange(Directory.EnumerateFiles(
                        LauncherViewModel.BaseGame.GameDirectory + "\\Data\\Scripts\\", "*.*",
                        SearchOption.AllDirectories));
                    files.AddRange(Directory.EnumerateFiles(LauncherViewModel.BaseGame.GameDirectory + "\\Data\\CustomMaps\\", "*.*", SearchOption.AllDirectories));

                    foreach (var file in files)
                    {
                        var flag =
                            await
                                Task.Run(
                                    () =>
                                        RestoreVersionContainer.Files.Any(
                                            k =>
                                                k.Name.Equals(Path.GetFileName(file), StringComparison.InvariantCultureIgnoreCase) && k.TargetType == TargetType.Ai &&
                                                Path.GetFullPath(file).IndexOf(k.TargetPath, StringComparison.InvariantCultureIgnoreCase) >= 0), _mSource.Token);

                        Progress = Progress + i;
                        if (flag)
                            continue;
                        RestoreTable.Files.Add(RestoreFile.CreateDeleteFile(file, TargetType.Ai));
                    }
                }


                //Find unused files to delete (Mod Files)
                if (!Directory.Exists(LauncherViewModel.CurrentMod.ModDirectory))
                    return true;
                foreach (
                    var file in
                        await
                            Task.Run(
                                () =>
                                    Directory.EnumerateFiles(LauncherViewModel.CurrentMod.ModDirectory, "*.*",
                                        SearchOption.AllDirectories), _mSource.Token))
                {
                    if (shallIgnore && RestoreHelper.IgnoreFile(file))
                        continue;

                    var item =
                        await
                            Task.Run(
                                () =>
                                    RestoreVersionContainer.Files.Find(
                                        k =>
                                            k.Name == Path.GetFileName(file) && k.TargetType == TargetType.Mod &&
                                            Path.GetFullPath(file).IndexOf(k.TargetPath, StringComparison.CurrentCultureIgnoreCase) >= 0), _mSource.Token);




                    Progress = Progress + i;
                    if (item != null)
                        continue;
                    RestoreTable.Files.Add(RestoreFile.CreateDeleteFile(file, TargetType.Mod));
                }
            }
            catch (TaskCanceledException)
            {
                Show(GetMessage("RestoreAborted"));
                return false;
            }
            return true;
        }

        /// <summary>
        /// Fille RestoreTable with files taht need to be downloaded
        /// </summary>
        /// <param name="excludeList"></param>
        /// <returns></returns>
        private async Task<bool> AddDownloadFilesToRestoreTable(List<string> excludeList)
        {
            if (!Version.TryParse(DataSource.DisplayedItem.Text, out Version version))
                throw new ArgumentException();

            RestoreTable = new RestoreTable(version);
            if (version != RestoreVersionContainer.Version)
                throw new Exception(GetMessage("ExceptionRestoreVersionNotMatch"));

            var hashProvider = new HashProvider();
            var listToCheck = RestoreVersionContainer.Files;
            if (excludeList != null)
                listToCheck = FileContainerFile.ListFromExcludeList(RestoreVersionContainer.Files, excludeList);

            var i = (double) 100/listToCheck.Count;

            try
            {
                //Find missing/corrupted files to download
                ProzessStatus = GetMessage("RestoreStatusCheckMissing");
                var t = listToCheck.Select(file => Task.Run(async () =>
                {
                    var absolutePath = CreateAbsoluteFilePath(file);
                    if (!await Task.Run(() => File.Exists(absolutePath), _mSource.Token) ||
                        await Task.Run(() => hashProvider.GetFileHash(absolutePath) != file.Hash, _mSource.Token))
                    {
                        var restoreFile = RestoreFile.CreateResotreFile(file, FileAction.Download);
                        RestoreTable.Files.Add(restoreFile);
                    }
                    Progress = Progress + i;
                }));

                await Task.WhenAll(t.ToArray());


                //foreach (var file in listToCheck)
                //{
                //    var absolutePath = CreateAbsoluteFilePath(file);
                //    if (!File.Exists(absolutePath))
                //    {
                //        var restoreFile = RestoreFile.CreateResotreFile(file, FileAction.Download);
                //        RestoreTable.Files.Add(restoreFile);
                //    }
                //    else if (hashProvider.GetFileHash(absolutePath) != file.Hash)
                //    {
                //        var restoreFile = RestoreFile.CreateResotreFile(file, FileAction.Download);
                //        RestoreTable.Files.Add(restoreFile);
                //    }
                //    Progress = Progress + i;
                //}




            }
            catch (TaskCanceledException)
            {
                Show(GetMessage("RestoreAborted"));
                return false;
            }
            finally
            {
                if (HostServer.HasErrors)
                    HostServer.ShowLog();
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
                return Path.GetFullPath(LauncherViewModel.BaseGame.GameDirectory + file.TargetPath);
            return Path.GetFullPath(LauncherViewModel.CurrentMod.ModDirectory + file.TargetPath);
        }

        private string CreateLocalFilePath(RestoreFile file)
        {
            if (file.TargetType == TargetType.Ai)
                return Path.Combine(LauncherViewModel.BaseGame.GameDirectory, file.TargetPath);
            return Path.Combine(LauncherViewModel.CurrentMod.ModDirectory, file.TargetPath);
        }

        /// <summary>
        /// Deletes all files marked in the RestoreTable
        /// </summary>
        /// <returns></returns>
        private async Task DeleteUnneededFiles()
        {
            foreach (var file in await Task.Run(() => RestoreTable.GetFilesOfAction(FileAction.Delete)))
            {
                var deletePath = CreateLocalFilePath(file);
                File.Delete(deletePath);
            }
        }

        /// <summary>
        /// Downloads the marked files in RestoreTable
        /// </summary>
        /// <returns></returns>
        private async Task<bool> DownloadRestoreFiles()
        {
            var filesToDownload = RestoreTable.GetFilesOfAction(FileAction.Download);
            var i = (double) 100/filesToDownload.Count;
            try
            {
                var t = filesToDownload.Select(file => Task.Run(async () =>
                {
                    if (!ComputerHasInternetConnection())
                    {
                        Show(GetMessage("RestoreInternetLost"));
                        return;
                    }
                    var restorePath = CreateAbsoluteFilePath(file);
                    await
                        Task.Run(() => HostServer.DownloadFile(file.SourcePath, restorePath),
                            _mSource.Token);
                    ProzessStatus = GetMessage("RestoreStatusDownloaded", file.Name);
                    Progress = Progress + i;
                }));

                await Task.WhenAll(t.ToArray());
            }
            catch (TaskCanceledException)
            {
                Show(GetMessage("RestoreAborted"));
                return false;
            }
            return true;
        }
      
        /// <summary>
        /// Main Procedure to Download and Delete marked files
        /// </summary>
        /// <returns></returns>
        private async Task<bool> InternalRestore()
        {
            if (RestoreTable == null)
            {
                Show(GetMessage("RestoreTableNull"));
                return false;
            }
            if (!await DownloadRestoreFiles())
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
            RestoreVersionFileStream = Stream.Null;
            RestoreVersionContainer = null;
            RestoreTable = null;
        }

        #region Hard

        /// <summary>
        /// Prepare the Hard-Restore
        /// </summary>
        /// <returns>True if was successful</returns>
        private async Task<bool> PrepareHardRestore()
        {
            await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);
            ProzessStatus = GetMessage("RestoreStautsDeletingMod");
            LauncherViewModel.BaseGame.DeleteMod(LauncherViewModel.CurrentMod.FolderName);
            LauncherViewModel.BaseGame.ClearDataFolder();

            await AnimateProgressBar(Progress, 50, 10, this, x => x.Progress);
            ProzessStatus = GetMessage("RestoreStatusPrepareTable");
            FillRestoreTableHard();
            await AnimateProgressBar(Progress, 101, 10, this, x => x.Progress);
            return true;
        }

        /// <summary>
        /// Deletes all files and fills the RestoreTable
        /// </summary>
        private void FillRestoreTableHard()
        {
            if (!Version.TryParse(DataSource.DisplayedItem.Text, out Version version))
                throw new ArgumentException();

            RestoreTable = new RestoreTable(version);
            if (version != RestoreVersionContainer.Version)
                throw new Exception(GetMessage("ExceptionRestoreVersionNotMatch"));
            foreach (
                var restoreFile in
                    RestoreVersionContainer.Files.Select(
                        file => RestoreFile.CreateResotreFile(file, FileAction.Download)))
            {
                RestoreTable.Files.Add(restoreFile);
            }
        }

        #endregion

        #region None

        private async Task<bool> PrepareNormalRestore()
        {
            await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);
            ProzessStatus = GetMessage("RestoreStatusPrepareTable");
            if (!await FillRestoreTableNormal())
                return false;
            return true;
        }

        private async Task<bool> FillRestoreTableNormal()
        {
            if (!await AddDownloadFilesToRestoreTable(null))
                return false;
            if (!await AddDeleteFilesToRestoreTable(false))
                return false;
            return true;
        }

        #endregion

        #region Igrnore Language

        private async Task<bool> PrepareLanguageIgnoreRestore()
        {
            await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);
            ProzessStatus = GetMessage("RestoreStatusPrepareTable");
            if (!await FillRestoreTableIgnoreLanguage())
                return false;
            return true;
        }

        private async Task<bool> FillRestoreTableIgnoreLanguage()
        {
            if (
                !await
                    AddDownloadFilesToRestoreTable(new List<string>
                    {
                        @"\Data\Audio\Speech\*",
                        @"\Data\*Speech.meg",
                        @"\Data\Text\",
                        @"\Data\Audio\"
                    }))
                return false;
            if (!await AddDeleteFilesToRestoreTable(true))
                return false;
            return true;
        }

        #endregion

        #region Commands

        public Command RestoreModCommand => new Command(RestoreMod);

        private void RestoreMod()
        {
            _mSource = new CancellationTokenSource();
            PerformRestore();
        }

        public Command CancelCommand => new Command(Cancel);

        private void Cancel()
        {
            _mSource?.Cancel(false);
        }

        public Command<object> ChangeSelectionCommand => new Command<object>(ChangeSelection, CanChangeSelection);

        private void ChangeSelection(object obj)
        {
            AudioHelper.PlayAudio(AudioHelper.Audio.ButtonPress);
            SelectedOption = (RestoreOptions) obj;
        }

        private bool CanChangeSelection(object arg)
        {
            return true;
        }

        #endregion

        #region RestoreXML

        /// <summary>
        /// Main Prcedure to get the RestoreXML Data
        /// </summary>
        /// <returns>False if failed</returns>
        private async Task<bool> GetRestoreVersionXmlData()
        {
            if (!await LoadRestoreVersionStream())
                return false;
            await AnimateProgressBar(Progress, 50, 1, this, x => x.Progress);
            if (!await Task.FromResult(ParseRestoreVersionXml()))
                return false;
            await AnimateProgressBar(Progress, 100, 1, this, x => x.Progress);
            return true;
        }

        /// <summary>
        /// Procedure to Download and Validate the XML File
        /// </summary>
        /// <returns>False if failed</returns>
        private async Task<bool> LoadRestoreVersionStream()
        {

            if (!Version.TryParse(DataSource.DisplayedItem.Text, out Version version))
                throw new ArgumentException();

            if (!HostServer.IsRunning())
            {
                Show(GetMessage("RestoreHostServerOffline"));
                return false;
            }
            if (
                !VersionUtilities.GetAllAvailableModVersionsOnline()
                    .Contains(new Version(version.ToString())))
            {
                Show(GetMessage("RestoreVersionNotMatch"));
                return false;
            }

            var downloadPath = LauncherViewModel.GetRescueFilePath(RestoreVersionFileFileName, true,
                version);

            await
                Task.Factory.StartNew(
                    () => RestoreVersionFileStream = HostServer.DownloadString(downloadPath).ToStream());

            if (RestoreVersionFileStream.IsEmpty())
            {
                Show(GetMessage("RestoreStreamNull"));
                return false;
            }

            var validator = new XmlValidator(Resources.FileContainer.ToStream());
            if (!validator.Validate(RestoreVersionFileStream))
            {
                Show(GetMessage("RestoreXmlNotValid"));
                return false;
            }
            return true;
        }

        /// <summary>
        /// Procedure to Parse the XMLFile into an Object
        /// </summary>
        /// <returns>False if failed</returns>
        private bool ParseRestoreVersionXml()
        {
            var parser = new XmlObjectParser<FileContainer>(RestoreVersionFileStream);
            RestoreVersionContainer = parser.Parse();
            return true;
        }

        #endregion
    }

    [Flags]
    public enum RestoreOptions
    {
        None = 1,
        IgnoreLanguage = 2,
        Hard = 4
    }
}