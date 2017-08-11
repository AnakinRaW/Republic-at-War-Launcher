using RawLauncher.Framework.Models;
using RawLauncher.Framework.ViewModels;

namespace RawLauncher.Framework.Helpers
{
    public static class CheckModHelper
    {
        internal static string GetReferenceDir(FileContainerFolder folder)
        {
            var rootDir = folder.TargetType == TargetType.Ai
                ? LauncherViewModel.FocStatic.GameDirectory
                : LauncherViewModel.CurrentModStatic.ModDirectory;

            var referenceDir = rootDir + folder.TargetPath;
            return referenceDir;
        }
    }
}
