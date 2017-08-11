using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ModernApplicationFramework.Input.Command;
using RawLauncherWPF.Helpers;
using RawLauncherWPF.Mods;
using RawLauncherWPF.UI;
using RawLauncherWPF.Utilities;
using static RawLauncherWPF.Utilities.MessageProvider;
using static RawLauncherWPF.Utilities.ProgressBarUtilities;
using static RawLauncherWPF.Utilities.VersionUtilities;

namespace RawLauncherWPF.ViewModels
{
    public sealed class UpdateViewModel : RestoreUpdateViewModel
    {
        private const string ChangelogFileName = "Changelog.txt";

        public UpdateViewModel(ILauncherPane pane) : base(pane)
        {
        }

        /// <summary>
        ///     Selected Restore Option
        /// </summary>
        private UpdateOptions SelectedOption { get; set; }


        public async Task<UpdateRestoreStatus> PerformUpdate()
        {
            var prepareResult = PrepareUpdateRestore(GetLatestModVersion());
            if (prepareResult != PrepareUpdateRestoreResult.Succeeded)
            {
                switch (prepareResult)
                {
                    case PrepareUpdateRestoreResult.NoInternet:
                        Show(GetMessage("UpdateNoInternet"));
                        break;
                }
                return UpdateRestoreStatus.Error;
            }

            ProzessStatus = GetMessage("UpdateStatusPrepare");


            if (LauncherViewModel.CurrentMod == null)
                LauncherViewModel.CurrentMod = new DummyMod();
            var l = LauncherViewModel.CurrentMod.InstalledLanguage;

            await AnimateProgressBar(Progress, 10, 0, this, x => x.Progress);

            var getXmlResult = await GetXmlData(GetLatestModVersion());
            if (getXmlResult != LoadRestoreUpdateResult.Suceeded)
            {
                switch (getXmlResult)
                {
                    case LoadRestoreUpdateResult.Offline:
                        GetMessage("UpdateHostOffline");
                        break;
                    case LoadRestoreUpdateResult.WrongVersion:
                        GetMessage("UpdateVersionNotFound");
                        break;
                    case LoadRestoreUpdateResult.StreamEmpty:
                        GetMessage("UpdateStreamNull");
                        break;
                    case LoadRestoreUpdateResult.StreamBroken:
                        GetMessage("UpdateXmlNotValid");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return UpdateRestoreStatus.Error;
            }

            await ThreadUtilities.SleepThread(250);
            await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);

            switch (SelectedOption)
            {
                case UpdateOptions.None:
                case 0:
                    var result = await PrepareNormalUpdate();
                    if (result != UpdateRestoreStatus.Succeeded)
                    {
                        return result;
                    }
                    break;
                case UpdateOptions.IgnoreVoice:
                    break;
                default:
                    result = await PrepareVoiceIgnoreUpdate();
                    if (result != UpdateRestoreStatus.Succeeded)
                    {
                        return result;
                    }
                    break;
            }
            await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);

            var internalRestoreResult = await InternalRestoreUpdate();
            if (internalRestoreResult != UpdateRestoreStatus.Succeeded)
            {
                if (internalRestoreResult == UpdateRestoreStatus.Error)
                    Show(GetMessage("UpdateTableNull"));
                return UpdateRestoreStatus.Error;
            }

            if (LauncherViewModel.CurrentMod is DummyMod)
                LauncherViewModel.CurrentMod = new RaW().FindMod();

            LauncherPane.MainWindowViewModel.InstalledVersion = LauncherViewModel.CurrentMod.Version;
            await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);
            ProzessStatus = "UpdateStatusFinishing";
            await Task.Run(() =>
            {
                var model = LauncherPane.MainWindowViewModel.LauncherPanes[2].ViewModel;
                var languageModel = (LanguageViewModel) model;
                languageModel?.ChangeLanguage(l);
            });
            await AnimateProgressBar(Progress, 101, 10, this, x => x.Progress);
            return UpdateRestoreStatus.Succeeded;
        }

        protected override void ResetUi()
        {
            base.ResetUi();
            if (!(LauncherViewModel.CurrentMod is DummyMod))
                return;
            LauncherPane.MainWindowViewModel.IsBlocked = true;
            CanExecute = true;
        }

        protected override bool AskUserToContinue() => UpdateHelper.AskUserToContinue();

        private async Task<UpdateRestoreStatus> PrepareNormalUpdate()
        {
            await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);
            ProzessStatus = "";
            return await FillUpdateTableNormal();
        }

        private async Task<UpdateRestoreStatus> FillUpdateTableNormal()
        {
            var result = await AddDownloadFilesToRestoreTable(GetLatestModVersion(), null);
            if (result != UpdateRestoreStatus.Succeeded)
            {
                if (result == UpdateRestoreStatus.Error)
                    Show(GetMessage("ExceptionUpdateVersionNotMatch"));
                return result;
            }
            return await AddDeleteFilesToRestoreTable(false);
        }

        private async Task<UpdateRestoreStatus> PrepareVoiceIgnoreUpdate()
        {
            await AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);
            ProzessStatus = GetMessage("UpdateStatusPrepare");
            return await FillUpdateTableVoice();
        }

        private async Task<UpdateRestoreStatus> FillUpdateTableVoice()
        {
            var result = await AddDownloadFilesToRestoreTable(GetLatestModVersion(), new List<string>
            {
                @"\Data\Audio\Speech\*",
                @"\Data\*Speech.meg",
                @"\Data\Audio\"
            });
            if (result != UpdateRestoreStatus.Succeeded)
            {
                if (result == UpdateRestoreStatus.Error)
                    Show(GetMessage("ExceptionRestoreVersionNotMatch"));
                return result;
            }
            return await AddDeleteFilesToRestoreTable(false);
        }

        public Command UpdateModCommand => new Command(UpdateMod);

        private async void UpdateMod()
        {
            PrepareUi();
            MSource = new CancellationTokenSource();
            var result = await PerformUpdate();
            if (result == UpdateRestoreStatus.Succeeded)
                Show(GetMessage("UpdateDone"));
            else if (result == UpdateRestoreStatus.Canceled)
                Show(GetMessage("UpdateAborted"));
            ResetUi();
        }

        protected override RestoreUpdateOperation ViewModelOperation => RestoreUpdateOperation.Update;

        protected override void ChangeSelection(object obj)
        {
            base.ChangeSelection(obj);
            SelectedOption = (UpdateOptions) obj;
        }

        public Command OpenChangelogCommand => new Command(OpenChangelog);

        private async void OpenChangelog()
        {
            await Task.Run(() => HostServer.DownloadFile("master\\" + ChangelogFileName,
                LauncherViewModel.RestoreDownloadDir + ChangelogFileName));
            var process = new Process
            {
                StartInfo = {FileName = LauncherViewModel.RestoreDownloadDir + ChangelogFileName}
            };
            process.Start();
        }
    }

    [Flags]
    public enum UpdateOptions
    {
        None = 1,
        IgnoreVoice = 2
    }
}