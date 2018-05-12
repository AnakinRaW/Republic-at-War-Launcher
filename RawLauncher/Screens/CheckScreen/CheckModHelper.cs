using Caliburn.Micro;
using RawLauncher.Framework.Launcher;
using RawLauncher.Framework.Models;

namespace RawLauncher.Framework.Screens.CheckScreen
{
    public static class CheckModHelper
    {
        internal static string GetReferenceDir(FileContainerFolder folder)
        {
            var launcher = IoC.Get<LauncherModel>();
            var rootDir = folder.TargetType == TargetType.Ai
                ? launcher.BaseGame.GameDirectory
                : launcher.CurrentMod.ModDirectory;

            var referenceDir = rootDir + folder.TargetPath;
            return referenceDir;
        }
    }
}
