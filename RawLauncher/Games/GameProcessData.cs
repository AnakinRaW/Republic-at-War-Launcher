using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using RawLauncher.Framework.Annotations;

namespace RawLauncher.Framework.Games
{
    public class GameProcessData : INotifyPropertyChanged
    {
        public GameProcessData()
        {
            IsProcessRunning = false;
        }

        public bool IsProcessRunning
        {
            get { return _isProcessRunning; }
            private set
            {
                if (value == _isProcessRunning)
                    return;
                _isProcessRunning = value;
                OnPropertyChanged();
            }
        }


        private Process _process;
        private bool _isProcessRunning;

        public Process Process
        {
            get { return _process; }
            set
            {
                if (Equals(value, _process))
                    return;
                if (_process != null)
                    _process.Exited -= Process_Exited;
                if (value == null)
                    return;
                _process = value;
                _process.EnableRaisingEvents = true;
                OnPropertyChanged();
                IsProcessRunning = ProcessHelper.IsProcessWithNameAlive(_process);
                _process.Exited += Process_Exited;
            }
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            IsProcessRunning = false;
        }

        public void StartProcess()
        {
            if (Process == null)
            {
                IsProcessRunning = false;
                return;
            }

            var fileName = Process.StartInfo.FileName;
            var wd = Process.StartInfo.WorkingDirectory;
            var a = Process.StartInfo.Arguments;

            CreateShortcut(fileName, wd, a);

            Process = new Process
            {
                StartInfo = {FileName = Path.Combine(wd, "tmp.lnk") }
            };
            Process.Start();
            IsProcessRunning = true;
        }

        private static void CreateShortcut(string filePath, string path ,string arguments)
        {
            var link = (NativeMethods.NativeMethods.IShellLink) new NativeMethods.NativeMethods.ShellLink();

            link.SetPath(filePath);
            link.SetWorkingDirectory(AppDomain.CurrentDomain.BaseDirectory);
            link.SetArguments(arguments);
            var exeFile = Path.Combine(Directory.GetCurrentDirectory(), "RawLauncher.Theme.dll");
            link.SetIconLocation(exeFile, 0);
            var file = (IPersistFile)link;
            file.Save(Path.Combine(path, "tmp.lnk"), false);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
