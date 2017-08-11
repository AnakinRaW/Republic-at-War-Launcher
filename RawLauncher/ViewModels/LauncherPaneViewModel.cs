using ModernApplicationFramework.Core;
using RawLauncher.Framework.UI;

namespace RawLauncher.Framework.ViewModels
{
    public abstract class LauncherPaneViewModel : ViewModelBase
    {
        private bool _isActive;
        private bool _isWorking;
        private bool _isBlocking;
        protected ILauncherPane LauncherPane;

        private bool _canExecute;

        protected LauncherPaneViewModel(ILauncherPane pane)
        {
            LauncherPane = pane;
            CanExecute = true;
        }

        /// <summary>
        /// Tells if the Pane is Active and showing on the MainWindow
        /// </summary>
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (Equals(value, _isActive))
                    return;
                _isActive = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Tells if the Pane is performing some Work
        /// </summary>
        public bool IsWorking
        {
            get => _isWorking;
            set
            {
                if (Equals(value, _isWorking))
                    return;
                _isWorking = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Tells if the Pane is currently blocking other panes from doing tasks
        /// </summary>
        public bool IsBlocking
        {
            get => _isBlocking;
            set
            {
                if (Equals(value, _isBlocking))
                    return;
                _isBlocking = value;
                LauncherPane.MainWindowViewModel.IsBlocked = _isBlocking;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Tells if the Pane can Execute a critical task
        /// </summary>
        public bool CanExecute
        {
            get => _canExecute;
            set
            {
                if (Equals(value, _canExecute))
                    return;
                _canExecute = value;
                OnPropertyChanged();
            }
        }
    }
}
