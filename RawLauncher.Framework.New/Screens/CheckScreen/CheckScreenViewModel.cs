using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading;
using System.Windows.Media;
using RawLauncher.Framework.Launcher;
using RawLauncher.Framework.Models;
using RawLauncher.Framework.Server;
using RawLauncher.Framework.Utilities;

namespace RawLauncher.Framework.Screens.CheckScreen
{
    [Export(typeof(ILauncherScreen))]
    [Export(typeof(ICheckScreen))]
    public class CheckScreenViewModel : LauncherScreen, ICheckScreen
    {
        private readonly LauncherModel _launcher;
        private readonly IHostServer _hostServer;
        private ImageSource _gameFoundIndicator;
        private string _gameFoundMessage;
        private string _gamesPatched;
        private ImageSource _gamesPatchedIndicator;
        private string _modAi;
        private ImageSource _modAiIndicator;
        private string _modFiles;
        private ImageSource _modFilesIndicator;
        private ImageSource _modFoundIndicator;
        private string _modFoundMessage;
        private CancellationTokenSource _mSource;
        private double _progress;
        private string _progressStatus;
        private readonly MessageRecorder _messageRecorder;

        public ImageSource GameFoundIndicator
        {
            get => _gameFoundIndicator;
            set
            {
                if (Equals(value, _gameFoundIndicator))
                    return;
                _gameFoundIndicator = value;
                NotifyOfPropertyChange();
            }
        }

        public string GameFoundMessage
        {
            get => _gameFoundMessage;
            set
            {
                if (Equals(value, _gameFoundMessage))
                    return;
                _gameFoundMessage = value;
                NotifyOfPropertyChange();
            }
        }

        public ImageSource GamesPatchedIndicator
        {
            get => _gamesPatchedIndicator;
            set
            {
                if (Equals(value, _gamesPatchedIndicator))
                    return;
                _gamesPatchedIndicator = value;
                NotifyOfPropertyChange();
            }
        }

        public string GamesPatchedMessage
        {
            get => _gamesPatched;
            set
            {
                if (Equals(value, _gamesPatched))
                    return;
                _gamesPatched = value;
                NotifyOfPropertyChange();
            }
        }

        public string ModAiMessage
        {
            get => _modAi;
            set
            {
                if (Equals(value, _modAi))
                    return;
                _modAi = value;
                NotifyOfPropertyChange();
            }
        }

        public ImageSource ModAiIndicator
        {
            get => _modAiIndicator;
            set
            {
                if (Equals(value, _modAiIndicator))
                    return;
                _modAiIndicator = value;
                NotifyOfPropertyChange();
            }
        }

        public ImageSource ModFoundIndicator
        {
            get => _modFoundIndicator;
            set
            {
                if (Equals(value, _modFoundIndicator))
                    return;
                _modFoundIndicator = value;
                NotifyOfPropertyChange();
            }
        }

        public string ModFoundMessage
        {
            get => _modFoundMessage;
            set
            {
                if (Equals(value, _modFoundMessage))
                    return;
                _modFoundMessage = value;
                NotifyOfPropertyChange();
            }
        }

        public string ModFilesMessage
        {
            get => _modFiles;
            set
            {
                if (Equals(value, _modFiles))
                    return;
                _modFiles = value;
                NotifyOfPropertyChange();
            }
        }

        public ImageSource ModFilesIndicator
        {
            get => _modFilesIndicator;
            set
            {
                if (Equals(value, _modFilesIndicator))
                    return;
                _modFilesIndicator = value;
                NotifyOfPropertyChange();
            }
        }

        public double Progress
        {
            get => _progress;
            set
            {
                if (Equals(value, _progress))
                    return;
                _progress = value;
                NotifyOfPropertyChange();
            }
        }

        public string ProzessStatus
        {
            get => _progressStatus;
            set
            {
                if (Equals(value, _progressStatus))
                    return;
                _progressStatus = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Contains AI Folder Information
        /// </summary>
        private List<FileContainerFolder> AiFolderList { get; set; }

        /// <summary>
        /// Stream which contains the XML data
        /// </summary>
        private Stream CheckFileStream { get; set; }

        /// <summary>
        /// Container with all information extracted from XML File/Stream
        /// </summary>
        private FileContainer FileContainer { get; set; }

        /// <summary>
        /// Contains Mod Folder Information
        /// </summary>
        private List<FileContainerFolder> ModFolderList { get; set; }

        [ImportingConstructor]
        public CheckScreenViewModel(LauncherModel launcher, IHostServer hostServer)
        {
            _launcher = launcher;
            _hostServer = hostServer;

            CheckFileStream = Stream.Null;
            _messageRecorder = new MessageRecorder();
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            GameFoundIndicator = IndicatorImagesHelper.SetColor(IndicatorImagesHelper.IndicatorColor.Blue);
            ModFoundIndicator = IndicatorImagesHelper.SetColor(IndicatorImagesHelper.IndicatorColor.Blue);
            GamesPatchedIndicator = IndicatorImagesHelper.SetColor(IndicatorImagesHelper.IndicatorColor.Blue);
            ModAiIndicator = IndicatorImagesHelper.SetColor(IndicatorImagesHelper.IndicatorColor.Blue);
            ModFilesIndicator = IndicatorImagesHelper.SetColor(IndicatorImagesHelper.IndicatorColor.Blue);
        }
    }
}
