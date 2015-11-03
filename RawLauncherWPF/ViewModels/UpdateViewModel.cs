using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ModernApplicationFramework.Commands;
using RawLauncherWPF.Models;
using RawLauncherWPF.Server;
using RawLauncherWPF.UI;
using RawLauncherWPF.Utilities;

namespace RawLauncherWPF.ViewModels
{
    public sealed class UpdateViewModel : LauncherPaneViewModel
    {
        private double _progress;
        private string _progressStatus;
        private CancellationTokenSource _mSource;
        private const string ChangelogFileName = "Changelog.txt";

        public UpdateViewModel(ILauncherPane pane) : base(pane)
        {
            LauncherViewModel = LauncherPane.MainWindowViewModel.LauncherViewModel;
        }

        /// <summary>
        /// Reference to the HostServer
        /// </summary>
        private IHostServer HostServer => LauncherViewModel.HostServerStatic;

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
        /// Reference to the LauncherViewModel
        /// </summary>
        private LauncherViewModel LauncherViewModel { get; }

        /// <summary>
        /// Data which contains the informations to update the Mod
        /// </summary>
        private RestoreTable UpdateTable { get; set; }

        /// <summary>
        /// Container with all information extracted from Restore XML File/Stream
        /// </summary>
        private FileContainer UpdateVersionContainer { get; set; }

        /// <summary>
        /// Stream which contains the XML data of the version to restore
        /// </summary>
        private Stream UpdateVersionFileStream { get; set; }

        public async void PerformUpdate()
        {
            
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
            UpdateVersionFileStream = Stream.Null;
            UpdateVersionContainer = null;
            UpdateTable = null;
        }

        #region Commands
        public Command UpdateModCommand => new Command(UpdateMod);

        private void UpdateMod()
        {
            AudioHelper.PlayAudio(AudioHelper.Audio.ButtonPress);
            _mSource = new CancellationTokenSource();
            PerformUpdate();
        }

        public Command OpenChangelogCommand => new Command(OpenChangelog);

        private void OpenChangelog()
        {
            AudioHelper.PlayAudio(AudioHelper.Audio.ButtonPress);
            Task.Run(() => HostServer.DownloadFile("Versions/" + ChangelogFileName, LauncherViewModel.RestoreDownloadDir + ChangelogFileName));
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
}
