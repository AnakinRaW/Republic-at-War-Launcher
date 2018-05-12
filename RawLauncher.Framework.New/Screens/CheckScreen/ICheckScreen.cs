using System.Windows.Input;
using System.Windows.Media;

namespace RawLauncher.Framework.Screens.CheckScreen
{
    public interface ICheckScreen : ILauncherScreen, ICancelable, IHasProgressBar
    {
        ICommand CheckVersionCommand { get; }

        ICommand PatchGamesCommand { get; }
      
        ImageSource GameFoundIndicator { get; set; }

        ImageSource GamesPatchedIndicator { get; set; }

        ImageSource ModAiIndicator { get; set; }

        ImageSource ModFilesIndicator { get; set; }

        ImageSource ModFoundIndicator { get; set; }

        string GameFoundMessage { get; set; }

        string GamesPatchedMessage { get; set; }
        
        string ModAiMessage { get; set; }
        
        string ModFilesMessage { get; set; }
        
        string ModFoundMessage { get; set; }
    }
}