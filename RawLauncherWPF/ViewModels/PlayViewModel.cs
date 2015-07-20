using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using ModernApplicationFramework.Commands;
using RawLauncherWPF.UI;

namespace RawLauncherWPF.ViewModels
{
    public sealed class PlayViewModel : LauncherPaneViewModel
    {

        private int _currentSessions;

        public PlayViewModel(ILauncherPane pane) : base(pane)
        {
        }

        public int CurrentSessions
        {
            get { return _currentSessions; }
            set
            {
                if (Equals(value, _currentSessions))
                    return;
                _currentSessions = value;
                OnPropertyChanged();
            }
        }

        #region Command

        public Command PlayModCommand => new Command(PlayMod, CanPlayMod);

        private bool CanPlayMod()
        {
            return true;
        }

        private void PlayMod()
        {
            MessageBox.Show(LauncherPane.MainWindowViewModel.LauncherViewModel.Foc.GameDirectory);
        }

        public Command OrganizeGameCommand => new Command(OrganizeGame, CanOrganizeGame);

        private bool CanOrganizeGame()
        {
            throw new NotImplementedException();
        }

        private void OrganizeGame()
        {
            throw new NotImplementedException();
        }

        public Command<ToggleButton> ToggleFastLaunchCommand => new Command<ToggleButton>(ToggleFastLaunch);

        private Task ToggleFastLaunch(ToggleButton arg)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
