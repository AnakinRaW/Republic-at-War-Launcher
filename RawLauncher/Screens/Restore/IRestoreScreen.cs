using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using ModernApplicationFramework.Controls.ComboBox;
using ModernApplicationFramework.Interfaces;

namespace RawLauncher.Framework.Screens.Restore
{
    public interface IRestoreScreen : ILauncherScreen, IUpdateRestoreBase
    {
        ComboBoxDataSource DataSource { get; set; }

        ObservableCollection<IHasTextProperty> AvailableVersions { get; set; }

        ICommand RestoreModCommand { get; }

        Task<UpdateRestoreStatus> PerformRestore(Version version);
    }
}
