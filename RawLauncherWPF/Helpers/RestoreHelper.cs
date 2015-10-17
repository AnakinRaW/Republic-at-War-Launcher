using System.Collections.Generic;
using System.Linq;
using ModernApplicationFramework.Controls;
using RawLauncherWPF.ViewModels;
using static RawLauncherWPF.Utilities.VersionUtilities;

namespace RawLauncherWPF.Helpers
{
    public static class RestoreHelper
    {
        public static List<ComboBoxItem> CreateVersionItems()
        {
            var versions = GetAllAvailableVersionsOnline();
            return (from version in versions
                where version <= LauncherViewModel.CurrentModStatic.Version
                select new ComboBoxItem {Content = version, DataContext = version}).ToList();
        } 
    }
}
