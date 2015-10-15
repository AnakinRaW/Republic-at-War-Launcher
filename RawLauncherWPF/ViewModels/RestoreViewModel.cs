using System;
using RawLauncherWPF.UI;

namespace RawLauncherWPF.ViewModels
{
    public sealed class RestoreViewModel : LauncherPaneViewModel
    {
        public RestoreViewModel(ILauncherPane pane) : base(pane)
        {
        }
    }

    [Flags]
    public enum RestoreOptions
    {
        None = 1,
        IgnoreLanguage = 2,
        Hard = 4
    }
}
