using System;
using System.ComponentModel.Composition;
using System.IO;
using RawLauncher.Framework.Launcher;
using RawLauncher.Framework.Mods;

namespace RawLauncher.Framework.Versioning
{
    [Export(typeof(IModVersionWatcher))]
    internal class ModVersionWatcher : IModVersionWatcher
    {
        private readonly LauncherModel _launcher;
        private FileSystemWatcher _watcher;
        public event EventHandler<ModVersion> ModVersionChanged;

        [ImportingConstructor]
        public ModVersionWatcher(LauncherModel launcher)
        {
            _launcher = launcher;
            launcher.PropertyChanged += OnLauncherPropertyChanged;
        }

        private void OnLauncherPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(LauncherModel.CurrentMod))
                return;
            if (_launcher.CurrentMod is DummyMod)
            {
                OnModVersionChanged(_launcher.CurrentMod.Version);
                return;
            }
            RegisterFileSystemWatcher();
            RegisterEvents();
            OnModVersionChanged(_launcher.CurrentMod.Version);
        }

        private void RegisterEvents()
        {
            _watcher.Changed += _watcher_Changed;
            _watcher.Renamed += _watcher_Changed;
            _watcher.Created += _watcher_Changed;
            _watcher.Deleted += _watcher_Changed;
        }

        private void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
            OnModVersionChanged(_launcher.CurrentMod.Version);
        }

        private void RegisterFileSystemWatcher()
        {
            if (_watcher != null)
            {
                _watcher.EnableRaisingEvents = false;
                _watcher.Dispose();
            }

            var filePath = Path.Combine(_launcher.CurrentMod.ModDirectory, @"Data\XML\Gameobjectfiles.xml");
            var watcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(filePath),
                Filter = Path.GetFileName(filePath),
                EnableRaisingEvents = true
            };

            _watcher = watcher;
        }

        protected virtual void OnModVersionChanged(ModVersion e)
        {
            ModVersionChanged?.Invoke(this, e);
        }
    }

    public interface IModVersionWatcher
    {
        event EventHandler<ModVersion> ModVersionChanged;
    }
}
