using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Interfaces;
using RawLauncher.Framework.Annotations;
using RawLauncher.Framework.Launcher;
using RawLauncher.Framework.Utilities;

namespace RawLauncher.Framework.Screens.Restore
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
            var versions = VersionUtilities.GetAllAvailableModVersionsOnline();
            var launcher = IoC.Get<LauncherModel>();
            if (launcher.CurrentMod == null)
                return null;

            var list = new ObservableCollection<IHasTextProperty>();
            foreach (var version in versions)
                if (version <= launcher.CurrentMod.Version)
                    list.Add(new VersionComboBoxItem(version.ToString()));
            return list;
        }

        /// <summary>
        /// Throw a message asking for confirmation about resetting the Mod
        /// </summary>
        /// <returns>True if User wants to continue</returns>
        public static bool AskUserToContinue()
        {
            var result = MessageProvider.Show(MessageProvider.GetMessage("RestoreOperationQuestion"), "Republic at War",
                MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No);
            return result != MessageBoxResult.No;
        }
    }
}
