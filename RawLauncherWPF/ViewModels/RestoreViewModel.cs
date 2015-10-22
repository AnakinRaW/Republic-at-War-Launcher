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
        private CancellationTokenSource _mSource;
        private int _progress;
        private string _progressStatus;
        private List<ComboBoxItem> _availableVersions;
        private ComboBoxItem _selectedVersion;

        public RestoreViewModel(ILauncherPane pane) : base(pane)
        {
            LauncherViewModel = LauncherPane.MainWindowViewModel.LauncherViewModel;
            AvailableVersions = RestoreHelper.CreateVersionItems();
            SelectedVersion = AvailableVersions.First();
        }

        /// <summary>
        /// Reference to the HostServer
        /// </summary>
        private IHostServer HostServer => LauncherViewModel.HostServerStatic;

        private LauncherViewModel LauncherViewModel { get; }

        /// <summary>
        /// Stream which contains the XML data of the version to restore
        /// </summary>
        private Stream RestoreVersionFileStream { get; set; }

        /// <summary>
        /// Container with all information extracted from Restore XML File/Stream
        /// </summary>
        private FileContainer RestoreVersionContainer { get; set; }

        private RestoreOptions SelectedOption { get; set; }

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

        private async void PerformRestore()
        {
            var result = Show("Are you sure you want to restore Republic at War ?\r\n" +
                 "Modified Files will be delted and restored with the original ones", "Republic at War",
                MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No);
            if (result == MessageBoxResult.No)
                return;
            PrepareUi();
            ProzessStatus = "Preparing Restore";
            if (!await GetRestoreVersionXmlData())
            {
                ResetUi();
                return;
            }
            //TODO: Perform individual tasks for selcted options
            await ThreadUtilities.SleepThread(250);
            await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);


            var parser = new XmlObjectParser<FileContainer>(RestoreVersionFileStream);

            var Container = parser.Parse();

           Show(Container.Files.First().Name);

            if (_mSource.IsCancellationRequested)
            {
                ResetUi();
                return;
            }

            //TODO: Rerform Restore

            ResetUi();
        }

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

        private bool ParseRestoreVersionXml()
        {
            var parser = new XmlObjectParser<FileContainer>(RestoreVersionFileStream);
            RestoreVersionContainer = parser.Parse();

            if (RestoreVersionContainer.Version != LauncherPane.MainWindowViewModel.LauncherViewModel.CurrentMod.Version)
            {
                Show("The Version of the mod does not match to the reference file. Please click Restore-Tab and let the launcher redownload the Files.");
                return false;
            }
            return true;
        }

        private async Task<bool> LoadRestoreVersionStream()
        {
            if (!HostServer.IsRunning())
            {
                Show("Could not Download the required files, because the servers are offline.\r\n" +
                     "Please try later");
                return false;
            }
            if (!VersionUtilities.GetAllAvailableVersionsOnline().Contains(LauncherViewModel.CurrentModStatic.Version))
            {
                Show("Your installed version is not available to check. Please try later or contact us.");
                return false;
            }
            await Task.Factory.StartNew(() => RestoreVersionFileStream = HostServer.DownloadString(LauncherViewModel.RescuePathGenerator(true) + RestoreVersionFileFileName).ToStream());

            if (RestoreVersionFileStream.IsEmpty())
            {
                Show("Error while downloading the required files.\r\n" +"Please try later");
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

        private void ResetUi()
        {
            IsWorking = false;
            IsBlocking = false;
        }

        private void PrepareUi()
        {
            Progress = 0;
            IsBlocking = true;
            IsWorking = true;
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