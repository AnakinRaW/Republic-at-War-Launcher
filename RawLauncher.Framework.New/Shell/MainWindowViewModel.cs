using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.WindowModels;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Interfaces.ViewModels;
using RawLauncher.Theme;

namespace RawLauncher.Framework.Shell
{
    [Export(typeof(IMainWindowViewModel))]
    public class MainWindowViewModel : MainWindowViewModelConductorOneActive
    {
        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            Application.Current.MainWindow = view as Window;

            IoC.Get<IThemeManager>().Theme = new LauncherTheme();
            IoC.Get<IStatusBarDataModelService>().SetVisibility(0);
        }

        protected override void OnViewReady(object view)
        {
            base.OnViewReady(view);
            IsSimpleWindow = true;
            UseSimpleMovement = true;
            UseTitleBar = false;
            UseMenu = false;
        }

        public override void OnImportsSatisfied()
        {
            base.OnImportsSatisfied();
            ActivateItem(null);
        }
    }
}
