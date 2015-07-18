using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModernApplicationFramework.ViewModels;
using RawLauncherWPF.UI;

namespace RawLauncherWPF.ViewModels
{
    class LauncherPaneViewModel : ViewModelBase
    {
        private bool _isActive;
        private bool _isWorking;
        private bool _isBlocking;

        protected ILauncherPane LauncherPane;

        public LauncherPaneViewModel(ILauncherPane pane)
        {
            LauncherPane = pane;
        }

        /// <summary>
        /// Tells if the Pane is Active and showing on the MainWindow
        /// </summary>
        public bool IsActive
        {
            get { return _isActive; }
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
            get { return _isWorking; }
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
            get { return _isBlocking; }
            set
            {
                if (Equals(value, _isBlocking))
                    return;
                _isBlocking = value;
                OnPropertyChanged();
            }
        }
    }
}
