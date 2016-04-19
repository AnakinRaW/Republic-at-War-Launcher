using System;
using ModernApplicationFramework.Core.Themes;

namespace RawLauncherWPF.Themes.LauncherTheme
{
    public class LauncherTheme : Theme
    {
        public override string Name { get; }

        public override Uri GetResourceUri()
        {
            return new Uri("/RawLauncherWPF;component/Themes/LauncherTheme/LauncherTheme.xaml", UriKind.Relative);
        }
    }
}
