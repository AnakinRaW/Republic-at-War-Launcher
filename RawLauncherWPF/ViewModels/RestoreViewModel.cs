using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using ModernApplicationFramework.Commands;
using ModernApplicationFramework.Controls;
using RawLauncherWPF.UI;
using RawLauncherWPF.Utilities;
using static RawLauncherWPF.Utilities.MessageProvider;

namespace RawLauncherWPF.ViewModels
{
    public sealed class RestoreViewModel : LauncherPaneViewModel
    {
        private CancellationTokenSource _mSource;
        private int _progress;
        private string _progressStatus;
        private List<ComboBoxItem> _availableVersions;
        private ComboBoxItem _selectedVersion;

        public RestoreViewModel(ILauncherPane pane) : base(pane)
        {
            var i = new List<ComboBoxItem>();
            i.Add(new ComboBoxItem {Name = "A", Content = "A"});
            i.Add(new ComboBoxItem { Name = "B", Content = "B" });
            AvailableVersions = i;
            SelectedVersion = AvailableVersions.First();

        }

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
            foreach (var s in VersionUtilities.GetAllAvailableVersionsOnline())
            {
                Show(s);
            }
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
    }

    [Flags]
    public enum RestoreOptions
    {
        None = 1,
        IgnoreLanguage = 2,
        Hard = 4
    }
}