using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using RawLauncherWPF.Annotations;

namespace RawLauncherWPF.Games
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
            Process?.Start();
            IsProcessRunning = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
