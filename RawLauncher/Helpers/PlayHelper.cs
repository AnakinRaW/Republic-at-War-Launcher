using System.Windows;
using RawLauncher.Framework.Games;
using RawLauncher.Framework.Mods;
using RawLauncher.Framework.Utilities;

namespace RawLauncher.Framework.Helpers
{
    public static class PlayHelper
    {
        public static bool Play(IGame game, IMod mod)
        {
            if (!game.IsGameAiClear())
            {
                var result = MessageBox.Show("Your AI is not installed correctly. Please press the 'Fix AI' button on the Check tab panel.\r\n " +
                                             "Do you want to continue?", "Republic at War", MessageBoxButton.YesNo, MessageBoxImage.Information,
                    MessageBoxResult.Yes);
                if (result == MessageBoxResult.No)
                    return true;
            }
            mod.PrepareStart(game);
            game.PlayGame(mod);
            ThreadUtilities.ThreadSaveShutdown();

            return false;
        }
    }
}
