using Caliburn.Micro;
using RawLauncher.Framework.Shell;

namespace RawLauncher.Framework.Screens
{
    public abstract class LauncherScreen : Screen, ILauncherScreen
    {
        private bool _isWorking;
        private bool _isBlocking;

        private bool _canExecute;

        protected LauncherScreen()
        {
            CanExecute = true;
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
                NotifyOfPropertyChange();
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
                IoC.Get<ILauncherMainWindow>().IsBlocked = _isBlocking;
                NotifyOfPropertyChange();
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
                NotifyOfPropertyChange();
            }
        }
    }
}
