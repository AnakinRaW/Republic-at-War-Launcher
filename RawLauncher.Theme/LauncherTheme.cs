using System;
using System.ComponentModel.Composition;
using System.Reflection;

namespace RawLauncher.Theme
{
    [Export(typeof(ModernApplicationFramework.Core.Themes.Theme))]
    public class LauncherTheme : ModernApplicationFramework.Core.Themes.Theme
    {
        public override string Name => Text;
        public override string Text => "RaWTheme";

        public override Uri GetResourceUri()
        {
            return new Uri("/RawLauncher.Theme;component/LauncherTheme.xaml", UriKind.Relative);
        }

        public static Version Version => Assembly.GetExecutingAssembly().GetName().Version;
    }
}