using System.Collections.Generic;
using ModernApplicationFramework.Controls;
using static RawLauncherWPF.Utilities.VersionUtilities;

namespace RawLauncherWPF.Helpers
{
    public static class RestoreHelper
    {
        public static List<ComboBoxItem> CreateVersionItems()
        {
            var list = new List<ComboBoxItem>();
            var versions = GetAllAvailableVersionsOnline();
            foreach (var version in versions)
            {
                //TODO: Do no allow versions higher than installed atm
                list.Add(new ComboBoxItem {Content = version, DataContext = version});
            }
            return list;
        } 
    }
}
