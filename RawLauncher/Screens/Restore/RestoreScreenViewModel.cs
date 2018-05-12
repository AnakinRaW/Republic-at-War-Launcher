using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Controls.ComboBox;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces;
using RawLauncher.Framework.Launcher;
using RawLauncher.Framework.Models;
using RawLauncher.Framework.Screens.LanguageScreen;
using RawLauncher.Framework.Server;
using RawLauncher.Framework.Shell;
using RawLauncher.Framework.Utilities;

namespace RawLauncher.Framework.Screens.Restore
{
    [Export(typeof(ILauncherScreen))]
    [Export(typeof(IRestoreScreen))]
    public class RestoreScreenViewModel : UpdateRestoreBase, IRestoreScreen
    {
        private ComboBoxDataSource _dataSource;
        private int _count;

        private RestoreOptions SelectedOption { get; set; }

        protected override RestoreUpdateOperation ViewModelOperation => RestoreUpdateOperation.Restore;

        public ComboBoxDataSource DataSource
        {
            get => _dataSource;
            set
            {
                if (Equals(value, _dataSource)) return;
                _dataSource = value;
                NotifyOfPropertyChange();
            }
        }

        public ObservableCollection<IHasTextProperty> AvailableVersions { get; set; }

        public ICommand RestoreModCommand => new Command(RestoreMod);

        [ImportingConstructor]
        public RestoreScreenViewModel(LauncherModel launcher, IHostServer server) : base(launcher, server)
        {
            if (!NativeMethods.NativeMethods.ComputerHasInternetConnection())
                return;
            AvailableVersions = RestoreHelper.CreateVersionItems();
            DataSource = new ComboBoxDataSource(AvailableVersions);

            DataSource.ChangeDisplayedItem(DataSource.Items.Count - 1);
        }

        public async Task<UpdateRestoreStatus> PerformRestore(Version version)
        {
            ProzessStatus = MessageProvider.GetMessage("RestoreStatusPrepare");
            var l = Launcher.CurrentMod.InstalledLanguage;

            if (version >= Version.Parse("1.1.5.1"))
            {
                Launcher.BaseGame.ClearDataFolder();
                Launcher.BaseGame.Patch();
            }

            await ProgressBarUtilities.AnimateProgressBar(Progress, 10, 0, this, x => x.Progress);

            var getXmlResult = await GetXmlData(version);
            if (getXmlResult != LoadRestoreUpdateResult.Suceeded)
            {
                switch (getXmlResult)
                {
                    case LoadRestoreUpdateResult.Offline:
                        MessageProvider.GetMessage("RestoreHostServerOffline");
                        break;
                    case LoadRestoreUpdateResult.WrongVersion:
                        MessageProvider.GetMessage("RestoreVersionNotMatch");
                        break;
                    case LoadRestoreUpdateResult.StreamEmpty:
                        MessageProvider.GetMessage("RestoreStreamNull");
                        break;
                    case LoadRestoreUpdateResult.StreamBroken:
                        MessageProvider.GetMessage("RestoreXmlNotValid");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return UpdateRestoreStatus.Error;
            }

            await ThreadUtilities.SleepThread(250);

            do
            {
                switch (SelectedOption)
                {
                    case RestoreOptions.None:
                    case 0:
                        var result = await PrepareNormalRestore();
                        if (result != UpdateRestoreStatus.Succeeded)
                            return result;
                        break;
                    case RestoreOptions.Hard:
                        await PrepareHardRestore();
                        break;
                    default:
                        result = await PrepareLanguageIgnoreRestore();
                        if (result != UpdateRestoreStatus.Succeeded)
                            return result;
                        break;
                }
                if (SelectedOption == RestoreOptions.Hard)
                    SelectedOption = RestoreOptions.None;

                if (RestoreTable.Files.Count == 0)
                    break;
                await ProgressBarUtilities.AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);

                var internalRestoreResult = await InternalRestoreUpdate();
                if (internalRestoreResult != UpdateRestoreStatus.Succeeded)
                {
                    if (internalRestoreResult == UpdateRestoreStatus.Error)
                        MessageProvider.Show(MessageProvider.GetMessage("RestoreTableNull"));
                    return UpdateRestoreStatus.Error;
                }
                await ProgressBarUtilities.AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);
                _count++;
            } while (_count <= 3);

            var windowModel = IoC.Get<ILauncherMainWindow>();
            ProzessStatus = MessageProvider.GetMessage("RestoreStatusFinishing");
            windowModel.InstalledVersion = Launcher.CurrentMod.Version;

            await Task.Run(() =>
            {
                IoC.Get<ILanguageScreen>()?.ChangeLanguage(l);
            });
            await ProgressBarUtilities.AnimateProgressBar(Progress, 101, 10, this, x => x.Progress);

            _count = 0;

            return UpdateRestoreStatus.Succeeded;
        }

        protected override bool AskUserToContinue()
        {
            return RestoreHelper.AskUserToContinue();
        }

        protected override void ChangeSelection(object obj)
        {
            base.ChangeSelection(obj);
            SelectedOption = (RestoreOptions)obj;
        }

        private async Task<UpdateRestoreStatus> PrepareHardRestore()
        {
            await ProgressBarUtilities.AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);
            ProzessStatus = MessageProvider.GetMessage("RestoreStautsDeletingMod");
            Launcher.BaseGame.DeleteMod(Launcher.CurrentMod);
            Launcher.BaseGame.ClearDataFolder();
            Launcher.BaseGame.Patch();

            await ProgressBarUtilities.AnimateProgressBar(Progress, 50, 10, this, x => x.Progress);
            ProzessStatus = MessageProvider.GetMessage("RestoreStatusPrepareTable");
            FillRestoreTableHard();
            await ProgressBarUtilities.AnimateProgressBar(Progress, 101, 10, this, x => x.Progress);
            return UpdateRestoreStatus.Succeeded;
        }

        private void FillRestoreTableHard()
        {
            if (!Version.TryParse(DataSource.DisplayedItem.Text, out Version version))
                throw new ArgumentException();

            RestoreTable = new RestoreTable(version);
            if (version != FileContainer.Version)
                throw new Exception(MessageProvider.GetMessage("ExceptionRestoreVersionNotMatch"));
            foreach (
                var restoreFile in
                FileContainer.Files.Select(
                    file => RestoreFile.CreateResotreFile(file, FileAction.Download)))
                RestoreTable.Files.Add(restoreFile);
        }

        private async Task<UpdateRestoreStatus> PrepareNormalRestore()
        {
            await ProgressBarUtilities.AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);
            ProzessStatus = MessageProvider.GetMessage("RestoreStatusPrepareTable");
            return await FillRestoreTableNormal();
        }

        private async Task<UpdateRestoreStatus> FillRestoreTableNormal()
        {
            if (!Version.TryParse(DataSource.DisplayedItem.Text, out Version version))
                throw new ArgumentException();
            var result = await AddDownloadFilesToRestoreTable(version, null);
            if (result != UpdateRestoreStatus.Succeeded)
            {
                if (result == UpdateRestoreStatus.Error)
                    MessageProvider.Show(MessageProvider.GetMessage("ExceptionRestoreVersionNotMatch"));
                return result;
            }
            return await AddDeleteFilesToRestoreTable(false);
        }

        private async Task<UpdateRestoreStatus> PrepareLanguageIgnoreRestore()
        {
            await ProgressBarUtilities.AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);
            ProzessStatus = MessageProvider.GetMessage("RestoreStatusPrepareTable");
            return await FillRestoreTableIgnoreLanguage();
        }

        private async Task<UpdateRestoreStatus> FillRestoreTableIgnoreLanguage()
        {
            if (!Version.TryParse(DataSource.DisplayedItem.Text, out var version))
                throw new ArgumentException();

            var result = await AddDownloadFilesToRestoreTable(version, new List<string>
            {
                @"Data\Audio\Speech\*",
                @"Data\*Speech.meg",
                @"Data\Text\",
                @"Data\Audio\",
                @"Data\Art\Movies\Binked\"
            });
            if (result != UpdateRestoreStatus.Succeeded)
            {
                if (result == UpdateRestoreStatus.Error)
                    MessageProvider.Show(MessageProvider.GetMessage("ExceptionRestoreVersionNotMatch"));
                return result;
            }
            return await AddDeleteFilesToRestoreTable(true);
        }

        private async void RestoreMod()
        {
            MSource = new CancellationTokenSource();
            Version.TryParse(DataSource.DisplayedItem?.Text, out Version version);

            var prepareResult = PrepareUpdateRestore(version);
            if (prepareResult != PrepareUpdateRestoreResult.Succeeded)
            {
                switch (prepareResult)
                {
                    case PrepareUpdateRestoreResult.NoInternet:
                        MessageProvider.Show(MessageProvider.GetMessage("RestoreNoInternet"));
                        break;
                    case PrepareUpdateRestoreResult.NoVersion:
                        MessageProvider.Show(MessageProvider.GetMessage("RestoreNoVersion"));
                        break;
                }
                return;
            }

            PrepareUi();

            var result = await PerformRestore(version);
            if (result == UpdateRestoreStatus.Succeeded)
                MessageProvider.Show(MessageProvider.GetMessage("RestoreDone"));
            else if (result == UpdateRestoreStatus.Canceled)
                MessageProvider.Show(MessageProvider.GetMessage("RestoreAborted"));
            ResetUi();
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
