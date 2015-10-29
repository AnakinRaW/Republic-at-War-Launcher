using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ModernApplicationFramework.Commands;
using ModernApplicationFramework.Controls;
using RawLauncherWPF.ExtensionClasses;
using RawLauncherWPF.Helpers;
using RawLauncherWPF.Models;
using RawLauncherWPF.Properties;
using RawLauncherWPF.Server;
using RawLauncherWPF.UI;
using RawLauncherWPF.Utilities;
using RawLauncherWPF.Xml;
using static RawLauncherWPF.Utilities.MessageProvider;
using static RawLauncherWPF.Utilities.ProgressBarUtilities;

namespace RawLauncherWPF.ViewModels
{
    public sealed class RestoreViewModel : LauncherPaneViewModel
    {
        private const string RestoreVersionFileFileName = "RestoreModFileContainer.xml";
        private List<ComboBoxItem> _availableVersions;
        private CancellationTokenSource _mSource;
        private int _progress;
        private string _progressStatus;
        private ComboBoxItem _selectedVersion;

        public RestoreViewModel(ILauncherPane pane) : base(pane)
        {
            LauncherViewModel = LauncherPane.MainWindowViewModel.LauncherViewModel;
            AvailableVersions = RestoreHelper.CreateVersionItems();
            SelectedVersion = AvailableVersions.First();
        }

        /// <summary>
        /// Contains all ComboBoxItems with Versions
        /// </summary>
        public List<ComboBoxItem> AvailableVersions
        {
            get { return _availableVersions; }
            set
            {
                if (Equals(value, _availableVersions))
                    return;
                _availableVersions = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Progress from 0 to 100
        /// </summary>
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
        /// Selected Version as ComboBoxItem
        /// </summary>
        public ComboBoxItem SelectedVersion
        {
            get { return _selectedVersion; }
            set
            {
                if (Equals(value, _selectedVersion))
                    return;
                _selectedVersion = value;
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
        private async void PerformRestore()
        {
            if (!AskUserToContinue())
                return;

            PrepareUi();
            ProzessStatus = "Preparing Restore";
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
                    Show("None");
                    break;
                case RestoreOptions.Hard:
                    await PrepareHardRestore();
                    break;
                default:
                    Show("IgnoreLanguage");
                    break;
            }
            ResetUi();
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
        }

        #region Commands

        public Command RestoreModCommand => new Command(RestoreMod);

        private void RestoreMod()
        {
            AudioHelper.PlayAudio(AudioHelper.Audio.ButtonPress);
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

        #region Hard

        /// <summary>
        /// Prepare the Hard-Restore
        /// </summary>
        /// <returns>True if was successful</returns>
        private async Task<bool> PrepareHardRestore()
        {
            await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);
            ProzessStatus = "Deleting Mod Files";
            LauncherViewModel.Foc.DeleteMod(LauncherViewModel.CurrentMod.FolderName);
            LauncherViewModel.Foc.ClearDataFolder();

            await AnimateProgressBar(Progress, 50, 10, this, x => x.Progress);
            ProzessStatus = "Preparing download-table";

            await ThreadUtilities.SleepThread(1000);
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
            if (!HostServer.IsRunning())
            {
                Show("Could not Download the required files, because the servers are offline.\r\n" +
                     "Please try later");
                return false;
            }
            if (!VersionUtilities.GetAllAvailableVersionsOnline().Contains(new Version(SelectedVersion.DataContext.ToString())))
            {
                Show("Your installed version is not available to check. Please try later or contact us.");
                return false;
            }

            var downloadPath = LauncherViewModel.GetRescueFilePath(RestoreVersionFileFileName, true, (Version)SelectedVersion.DataContext);

            await
                Task.Factory.StartNew(
                    () => RestoreVersionFileStream = HostServer.DownloadString(downloadPath).ToStream());

            if (RestoreVersionFileStream.IsEmpty())
            {
                Show("Error while downloading the required files.\r\n" + "Please try later");
                return false;
            }

            var validator = new XmlValidator(Resources.FileContainer.ToStream());
            if (!validator.Validate(RestoreVersionFileStream))
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
        private bool ParseRestoreVersionXml()
        {
            var parser = new XmlObjectParser<FileContainer>(RestoreVersionFileStream);
            RestoreVersionContainer = parser.Parse();
            return true;
        }

        #endregion

        /// <summary>
        /// Throw a message asking for confirmation about resetting the Mod
        /// </summary>
        /// <returns>True if User wants to continue</returns>
        private bool AskUserToContinue()
        {
            var result = Show("Are you sure you want to restore Republic at War ?\r\n" + "This cannot be undone\r\n" +
                              "Modified Files will be delted and restored with the original ones", "Republic at War",
                MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No);
            return result != MessageBoxResult.No;
        }
    }

    [Flags]
    public enum RestoreOptions
    {
        None = 1,
        IgnoreLanguage = 2,
        Hard = 4
    }
}