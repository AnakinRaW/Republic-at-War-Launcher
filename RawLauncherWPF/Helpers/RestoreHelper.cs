using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using ModernApplicationFramework.Interfaces;
using RawLauncherWPF.Annotations;
using RawLauncherWPF.ViewModels;
using static RawLauncherWPF.Utilities.MessageProvider;
using static RawLauncherWPF.Utilities.VersionUtilities;

namespace RawLauncherWPF.Helpers
{

    internal class VersionComboBoxItem : IHasTextProperty
    {
        private string _text;

        public string Text
        {
            get => _text;
            set
            {
                if (value == _text) return;
                _text = value;
                OnPropertyChanged();
            }
        }

        public VersionComboBoxItem(string text)
        {
            _text = text;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public static class RestoreHelper
    {
        public static ObservableCollection<IHasTextProperty> CreateVersionItems()
        {
            var versions = GetAllAvailableModVersionsOnline();
            if (LauncherViewModel.CurrentModStatic == null)
                return null;

            var list = new ObservableCollection<IHasTextProperty>();
            foreach (var version in versions)
                if (version <= LauncherViewModel.CurrentModStatic.Version)
                    list.Add(new VersionComboBoxItem(version.ToString()));
            return list;
        }

        /// <summary>
        /// Throw a message asking for confirmation about resetting the Mod
        /// </summary>
        /// <returns>True if User wants to continue</returns>
        public static bool AskUserToContinue()
        {
            var result = Show(GetMessage("RestoreOperationQuestion"), "Republic at War",
                MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No);
            return result != MessageBoxResult.No;
        }

        public static bool IgnoreFile(string file)
        {
            if (file == null)
                throw new NullReferenceException(nameof(file));
            if (Path.GetFullPath(file).Contains("\\Text\\"))
                return true;
            if (Path.GetFullPath(file).Contains("\\Audio\\Speech\\"))
                return true;
            var s = new FileInfo(file).Directory?.Name;
            if (s == "Audio")
                return true;
            if (Path.GetFileName(file).Contains("Speech.meg"))
                return true;
            return false;
        }
    }
}
