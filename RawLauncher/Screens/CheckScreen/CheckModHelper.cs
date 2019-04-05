using System;
using System.Collections.Generic;
using Caliburn.Micro;
using RawLauncher.Framework.Launcher;
using RawLauncher.Framework.Models;
using RawLauncher.Framework.Versioning;

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

        internal static IReadOnlyCollection<string> BuildExcludeList(ModVersion version)
        {
            //Default exludeList:
            //  All Language Speech Files
            //  All Files in Mod's root folder
            //  Text files
            //  All SFX Files
            //  All movies
            var list = new List<string>{@"\Data\Audio\Speech\*", @"\", @"\Data\Text\",
                @"\Data\Audio\", @"\Data\Art\Movies\Binked\"};

            if (version > ModVersion.Parse("1.2.0.1"))
                list.Add(@"\Data\UnitNames\");

            return list;
        }
    }
}
