using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RawLauncherWPF.ViewModels;

namespace RawLauncherWPF.UI
{
    /// <summary>
    /// Interaktionslogik für PlayPane.xaml
    /// </summary>
    public partial class PlayPane : ILauncherPane
    {
        public PlayPane(MainWindowViewModel mainWindowViewModel)
        {
            InitializeComponent();
            if (mainWindowViewModel == null)
                throw new NoNullAllowedException(nameof(mainWindowViewModel));
            MainWindowViewModel = mainWindowViewModel;
            DataContext = new PlayViewModel(this);
            ViewModel = (LauncherPaneViewModel)DataContext;
        }

        public MainWindowViewModel MainWindowViewModel { get; }
        public LauncherPaneViewModel ViewModel { get; }
    }
}
