using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using ModernApplicationFramework.Commands;
using RawLauncherWPF.Games;
using RawLauncherWPF.UI;
using RawLauncherWPF.Utilities;
using static RawLauncherWPF.Utilities.IndicatorImagesHelper;

namespace RawLauncherWPF.ViewModels
{
    public sealed class CheckViewModel : LauncherPaneViewModel
    {
        private ImageSource _gameFoundIndicator;
        private string _gameFoundMessage;
        private string _gamesPatched;
        private ImageSource _gamesPatchedIndicator;
        private string _modAiCorrect;
        private ImageSource _modAiIndicator;
        private ImageSource _modFoundIndicator;
        private string _modFoundMessage;
        private string _modXmlCorrect;
        private ImageSource _modXmlIndicator;
        private int _progress;

        public CheckViewModel(ILauncherPane pane) : base(pane)
        {
            GameFoundIndicator = SetColor(IndicatorColor.Blue);
            ModFoundIndicator = SetColor(IndicatorColor.Blue);
            GamesPatchedIndicator = SetColor(IndicatorColor.Blue);
            ModAiIndicator = SetColor(IndicatorColor.Blue);
            ModXmlIndicator = SetColor(IndicatorColor.Blue);
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

        public int Progress
        {
            get { return _progress; }
            set
            {
                if (Equals(value, _progress))
                    return;
                _progress = value;
                OnPropertyChanged();
            }
        }

        private void CreatePatchMessage(bool eaw, bool foc)
        {
            if (eaw && foc)
                MessageBox.Show("Games successfuly patched.");
            else if (!eaw && !foc)
                MessageBox.Show("Games not successfuly patched.");
            else if (!eaw)
                MessageBox.Show("Foc successfuly patched.\r\nEaw not successfuly patched.");
            else
                MessageBox.Show("Foc not successfuly patched\r\nEaw successfuly patched");
        }

        private bool PatchGame(IGame game)
        {
            return game.Patch();
        }

        #region Commands

        public Command PatchGamesCommand => new Command(PatchGames);

        private void PatchGames()
        {
            AudioHelper.PlayAudio(AudioHelper.Audio.ButtonPress);
            var eaw = PatchGame(LauncherPane.MainWindowViewModel.LauncherViewModel.Eaw);
            var foc = PatchGame(LauncherPane.MainWindowViewModel.LauncherViewModel.Foc);
            CreatePatchMessage(eaw, foc);
        }

        public Command CheckVersionCommand => new Command(CheckVersion);

        async private void CheckVersion()
        {
            IsBlocking = true;
            IsWorking = true;

            PrepareForCheck();

            bool b = await Test1();

            MessageBox.Show(b.ToString());

            GameFoundIndicator = SetColor(IndicatorColor.Gray);

            IsWorking = false;
            IsBlocking = false;

        }

        private void Test3()
        {
            for (int i = 0; i < 101; i++)
            {
                Thread.Sleep(100);
                Progress++;
            }
        }


        async Task<bool> Test1()
        {
            await Task.Run(() =>
            {
                Thread.Sleep(2000);
                Test3();
            });
            return false;
        }


        private void PrepareForCheck()
        {
            GameFoundIndicator = SetColor(IndicatorColor.Blue);
            ModFoundIndicator = SetColor(IndicatorColor.Blue);
            GamesPatchedIndicator = SetColor(IndicatorColor.Blue);
            ModAiIndicator = SetColor(IndicatorColor.Blue);
            ModXmlIndicator = SetColor(IndicatorColor.Blue);
        }

        #endregion
    }
}