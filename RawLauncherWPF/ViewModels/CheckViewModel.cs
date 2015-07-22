using System.Windows.Media;
using ModernApplicationFramework.Commands;
using RawLauncherWPF.UI;
using static RawLauncherWPF.Utilities.ImageUtilities;

namespace RawLauncherWPF.ViewModels
{
    public sealed class CheckViewModel : LauncherPaneViewModel
    {
        private int _progress;
        private string _gameFoundMessage;
        private string _modFoundMessage;
        private string _gamesPatched;
        private string _modAiCorrect;
        private string _modXmlCorrect;

        private ImageSource _gameFoundIndicator;
        private ImageSource _modFoundIndicator;
        private ImageSource _gamesPatchedIndicator;
        private ImageSource _modAiIndicator;
        private ImageSource _modXmlIndicator;

        public CheckViewModel(ILauncherPane pane) : base(pane)
        {
            GameFoundIndicator = GetImageSourceFromPath("Resources/Visual/Check/BlueIndicator.png");
            ModFoundIndicator = GetImageSourceFromPath("Resources/Visual/Check/BlueIndicator.png");
            GamesPatchedIndicator = GetImageSourceFromPath("Resources/Visual/Check/BlueIndicator.png");
            ModAiIndicator = GetImageSourceFromPath("Resources/Visual/Check/BlueIndicator.png");
            ModXmlIndicator = GetImageSourceFromPath("Resources/Visual/Check/BlueIndicator.png");
        }

        public int Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                if (Equals(value, _progress))
                    return;
                _progress = value;
                OnPropertyChanged();
            }
        }

        public string GameFoundMessage
        {
            get { return _gameFoundMessage; }
            set
            {
                if (Equals(value, _gameFoundMessage))
                    return;
                _gameFoundMessage = value;
                OnPropertyChanged();
            }
        }

        public string ModFoundMessage
        {
            get { return _modFoundMessage; }
            set
            {
                if (Equals(value, _modFoundMessage))
                    return;
                _modFoundMessage = value;
                OnPropertyChanged();
            }
        }

        public string GamesPatched
        {
            get { return _gamesPatched; }
            set
            {
                if (Equals(value, _gamesPatched))
                    return;
                _gamesPatched = value;
                OnPropertyChanged();
            }
        }

        public string ModAiCorrect
        {
            get { return _modAiCorrect; }
            set
            {
                if (Equals(value, _modAiCorrect))
                    return;
                _modAiCorrect = value;
                OnPropertyChanged();
            }
        }

        public string ModXmlCorrect
        {
            get { return _modXmlCorrect; }
            set
            {
                if (Equals(value, _modXmlCorrect))
                    return;
                _modXmlCorrect = value;
                OnPropertyChanged();
            }
        }

        public ImageSource GameFoundIndicator
        {
            get { return _gameFoundIndicator; }
            set
            {
                if (Equals(value, _gameFoundIndicator))
                    return;
                _gameFoundIndicator = value;
                OnPropertyChanged();
            }
        }

        public ImageSource GamesPatchedIndicator
        {
            get { return _gamesPatchedIndicator; }
            set
            {
                if (Equals(value, _gamesPatchedIndicator))
                    return;
                _gamesPatchedIndicator = value;
                OnPropertyChanged();
            }
        }

        public ImageSource ModAiIndicator
        {
            get { return _modAiIndicator; }
            set
            {
                if (Equals(value, _modAiIndicator))
                    return;
                _modAiIndicator = value;
                OnPropertyChanged();
            }
        }

        public ImageSource ModXmlIndicator
        {
            get { return _modXmlIndicator; }
            set
            {
                if (Equals(value, _modXmlIndicator))
                    return;
                _modXmlIndicator = value;
                OnPropertyChanged();
            }
        }

        public ImageSource ModFoundIndicator
        {
            get { return _modFoundIndicator; }
            set
            {
                if (Equals(value, _modFoundIndicator))
                    return;
                _modFoundIndicator = value;
                OnPropertyChanged();
            }
        }


        #region Commands
        public Command PatchGamesCommand => new Command(PatchGames);

        private void PatchGames()
        {
            GameFoundIndicator = GetImageSourceFromPath("Resources/Visual/Check/RedIndicator.png");
            ModFoundIndicator = GetImageSourceFromPath("Resources/Visual/Check/RedIndicator.png");
            GamesPatchedIndicator = GetImageSourceFromPath("Resources/Visual/Check/RedIndicator.png");
            ModAiIndicator = GetImageSourceFromPath("Resources/Visual/Check/RedIndicator.png");
            ModXmlIndicator = GetImageSourceFromPath("Resources/Visual/Check/RedIndicator.png");
        }

        public Command CheckVersionCommand => new Command(CheckVersion);

        private void CheckVersion()
        {
        }

        #endregion

    }
}
