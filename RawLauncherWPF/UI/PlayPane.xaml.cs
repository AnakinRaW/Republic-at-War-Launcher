using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RawLauncherWPF.ViewModels;

namespace RawLauncherWPF.UI
{
    /// <summary>
    /// Interaktionslogik für PlayPane.xaml
    /// </summary>
    public partial class PlayPane : ILauncherPane
    {
        public PlayPane()
        {
            InitializeComponent();
            DataContext = new PlayViewModel(this);
            ViewModel = (LauncherPaneViewModel) DataContext;
        }

        public LauncherPaneViewModel ViewModel { get; }
    }
}
