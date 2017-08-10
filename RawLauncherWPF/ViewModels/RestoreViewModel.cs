using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Controls.ComboBox;
using ModernApplicationFramework.Interfaces;
using RawLauncherWPF.Helpers;
using RawLauncherWPF.Models;
using RawLauncherWPF.UI;
using RawLauncherWPF.Utilities;
using static RawLauncherWPF.NativeMethods.NativeMethods;
using static RawLauncherWPF.Utilities.MessageProvider;
using static RawLauncherWPF.Utilities.ProgressBarUtilities;

namespace RawLauncherWPF.ViewModels
{
    public sealed class RestoreViewModel : RestoreUpdateViewModel
    {
        private ComboBoxDataSource _dataSource;

        public RestoreViewModel(ILauncherPane pane) : base(pane)
        {
            if (!ComputerHasInternetConnection())
                return;
            AvailableVersions = RestoreHelper.CreateVersionItems();
            DataSource = new ComboBoxDataSource(AvailableVersions);
        }

        /// <summary>
        ///     Contains all ComboBoxItems with Versions
        /// </summary>
        public ObservableCollection<IHasTextProperty> AvailableVersions { get; set; }


        public ComboBoxDataSource DataSource
        {
            get => _dataSource;
            set
            {
                if (Equals(value, _dataSource)) return;
                _dataSource = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Selected Restore Option
        /// </summary>
        private RestoreOptions SelectedOption { get; set; }

        public Command RestoreModCommand => new Command(RestoreMod);

        protected override RestoreUpdateOperation ViewModelOperation => RestoreUpdateOperation.Restore;


        private int _count;

        /// <summary>
        ///     Main Procedure to Restore
        /// </summary>
        public async Task<UpdateRestoreStatus> PerformRestore(Version version)
        {
            ProzessStatus = GetMessage("RestoreStatusPrepare");
            var l = LauncherViewModel.CurrentMod.InstalledLanguage;
            await AnimateProgressBar(Progress, 10, 0, this, x => x.Progress);

            var getXmlResult = await GetXmlData(version);
            if (getXmlResult != LoadRestoreUpdateResult.Suceeded)
            {
                switch (getXmlResult)
                {
                    case LoadRestoreUpdateResult.Offline:
                        GetMessage("RestoreHostServerOffline");
                        break;
                    case LoadRestoreUpdateResult.WrongVersion:
                        GetMessage("RestoreVersionNotMatch");
                        break;
                    case LoadRestoreUpdateResult.StreamEmpty:
                        GetMessage("RestoreStreamNull");
                        break;
                    case LoadRestoreUpdateResult.StreamBroken:
                        GetMessage("RestoreXmlNotValid");
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
                await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);

                var internalRestoreResult = await InternalRestoreUpdate();
                if (internalRestoreResult != UpdateRestoreStatus.Succeeded)
                {
                    if (internalRestoreResult == UpdateRestoreStatus.Error)
                        Show(GetMessage("RestoreTableNull"));
                    return UpdateRestoreStatus.Error;
                }
                await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);
                _count++;
            } while (_count <= 3);
          
            ProzessStatus = GetMessage("RestoreStatusFinishing");
            LauncherPane.MainWindowViewModel.InstalledVersion = LauncherViewModel.CurrentMod.Version;

            await Task.Run(() =>
            {
                var model = LauncherPane.MainWindowViewModel.LauncherPanes[2].ViewModel;
                var languageModel = (LanguageViewModel) model;
                languageModel?.ChangeLanguage(l);
            });
            await AnimateProgressBar(Progress, 101, 10, this, x => x.Progress);

            _count = 0;

            return UpdateRestoreStatus.Succeeded;
        }


        /// <summary>
        ///     Prepare the Hard-Restore
        /// </summary>
        /// <returns>True if was successful</returns>
        private async Task<UpdateRestoreStatus> PrepareHardRestore()
        {
            await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);
            ProzessStatus = GetMessage("RestoreStautsDeletingMod");
            LauncherViewModel.BaseGame.DeleteMod(LauncherViewModel.CurrentMod.FolderName);
            LauncherViewModel.BaseGame.ClearDataFolder();

            await AnimateProgressBar(Progress, 50, 10, this, x => x.Progress);
            ProzessStatus = GetMessage("RestoreStatusPrepareTable");
            FillRestoreTableHard();
            await AnimateProgressBar(Progress, 101, 10, this, x => x.Progress);
            return UpdateRestoreStatus.Succeeded;
        }

        /// <summary>
        ///     Deletes all files and fills the RestoreTable
        /// </summary>
        private void FillRestoreTableHard()
        {
            if (!Version.TryParse(DataSource.DisplayedItem.Text, out Version version))
                throw new ArgumentException();

            RestoreTable = new RestoreTable(version);
            if (version != FileContainer.Version)
                throw new Exception(GetMessage("ExceptionRestoreVersionNotMatch"));
            foreach (
                var restoreFile in
                FileContainer.Files.Select(
                    file => RestoreFile.CreateResotreFile(file, FileAction.Download)))
                RestoreTable.Files.Add(restoreFile);
        }

        private async Task<UpdateRestoreStatus> PrepareNormalRestore()
        {
            await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);
            ProzessStatus = GetMessage("RestoreStatusPrepareTable");
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
                    Show(GetMessage("ExceptionRestoreVersionNotMatch"));
                return result;
            }
            return await AddDeleteFilesToRestoreTable(false);
        }

        private async Task<UpdateRestoreStatus> PrepareLanguageIgnoreRestore()
        {
            await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);
            ProzessStatus = GetMessage("RestoreStatusPrepareTable");
            return await FillRestoreTableIgnoreLanguage();
        }

        private async Task<UpdateRestoreStatus> FillRestoreTableIgnoreLanguage()
        {
            if (!Version.TryParse(DataSource.DisplayedItem.Text, out Version version))
                throw new ArgumentException();

            var result = await AddDownloadFilesToRestoreTable(version, new List<string>
            {
                @"\Data\Audio\Speech\*",
                @"\Data\*Speech.meg",
                @"\Data\Text\",
                @"\Data\Audio\"
            });
            if (result != UpdateRestoreStatus.Succeeded)
            {
                if (result == UpdateRestoreStatus.Error)
                    Show(GetMessage("ExceptionRestoreVersionNotMatch"));
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
                        Show(GetMessage("RestoreNoInternet"));
                        break;
                    case PrepareUpdateRestoreResult.NoVersion:
                        Show(GetMessage("RestoreNoVersion"));
                        break;
                }
                return;
            }

            PrepareUi();

            var result = await PerformRestore(version);
            if (result == UpdateRestoreStatus.Succeeded)
                Show(GetMessage("RestoreDone"));
            else if (result == UpdateRestoreStatus.Canceled)
                Show(GetMessage("RestoreAborted"));
            ResetUi();
        }

        protected override void ChangeSelection(object obj)
        {
            base.ChangeSelection(obj);
            SelectedOption = (RestoreOptions) obj;
        }

        protected override bool AskUserToContinue() => RestoreHelper.AskUserToContinue();
    }

    [Flags]
    public enum RestoreOptions
    {
        None = 1,
        IgnoreLanguage = 2,
        Hard = 4
    }
}