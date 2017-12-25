using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ModernApplicationFramework.Input.Command;
using RawLauncher.Framework.Helpers;
using RawLauncher.Framework.Mods;
using RawLauncher.Framework.UI;
using RawLauncher.Framework.Utilities;

namespace RawLauncher.Framework.ViewModels
{
    public sealed class UpdateViewModel : RestoreUpdateViewModel
    {
        private const string ChangelogFileName = "Changelog.txt";

        private int _count;

        public UpdateViewModel(ILauncherPane pane) : base(pane)
        {
        }

        /// <summary>
        ///     Selected Restore Option
        /// </summary>
        private UpdateOptions SelectedOption { get; set; }


        public async Task<UpdateRestoreStatus> PerformUpdate()
        {
            var prepareResult = PrepareUpdateRestore(VersionUtilities.GetLatestModVersion());
            if (prepareResult != PrepareUpdateRestoreResult.Succeeded)
            {
                switch (prepareResult)
                {
                    case PrepareUpdateRestoreResult.NoInternet:
                        MessageProvider.Show(MessageProvider.GetMessage("UpdateNoInternet"));
                        break;
                }
                return UpdateRestoreStatus.Error;
            }

            ProzessStatus = MessageProvider.GetMessage("UpdateStatusPrepare");


            if (LauncherViewModel.CurrentMod == null)
                LauncherViewModel.CurrentMod = new DummyMod(LauncherViewModel.BaseGame);
            var l = LauncherViewModel.CurrentMod.InstalledLanguage;

            //if (LauncherViewModel.CurrentMod.Version <= Version.Parse("1.1.5"))
            //{
                LauncherViewModel.BaseGame.ClearDataFolder();
                LauncherViewModel.BaseGame.Patch();
            //}

            await ProgressBarUtilities.AnimateProgressBar(Progress, 10, 0, this, x => x.Progress);

            var getXmlResult = await GetXmlData(VersionUtilities.GetLatestModVersion());
            if (getXmlResult != LoadRestoreUpdateResult.Suceeded)
            {
                switch (getXmlResult)
                {
                    case LoadRestoreUpdateResult.Offline:
                        MessageProvider.GetMessage("UpdateHostOffline");
                        break;
                    case LoadRestoreUpdateResult.WrongVersion:
                        MessageProvider.GetMessage("UpdateVersionNotFound");
                        break;
                    case LoadRestoreUpdateResult.StreamEmpty:
                        MessageProvider.GetMessage("UpdateStreamNull");
                        break;
                    case LoadRestoreUpdateResult.StreamBroken:
                        MessageProvider.GetMessage("UpdateXmlNotValid");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return UpdateRestoreStatus.Error;
            }

            await ThreadUtilities.SleepThread(250);
            await ProgressBarUtilities.AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);

            do
            {
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

                if (RestoreTable.Files.Count == 0)
                    break;

                await ProgressBarUtilities.AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);

                var internalRestoreResult = await InternalRestoreUpdate();
                if (internalRestoreResult != UpdateRestoreStatus.Succeeded)
                {
                    if (internalRestoreResult == UpdateRestoreStatus.Error)
                        MessageProvider.Show(MessageProvider.GetMessage("UpdateTableNull"));
                    return UpdateRestoreStatus.Error;
                }

                await ProgressBarUtilities.AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);
                _count++;
            } while (_count <= 3);

            if (LauncherViewModel.CurrentMod is DummyMod)
                LauncherViewModel.CurrentMod = new RaW().FindMod(LauncherViewModel.BaseGame);

            LauncherPane.MainWindowViewModel.InstalledVersion = LauncherViewModel.CurrentMod.Version;
            
            ProzessStatus = "UpdateStatusFinishing";
            await Task.Run(() =>
            {
                var model = LauncherPane.MainWindowViewModel.LauncherPanes[2].ViewModel;
                var languageModel = (LanguageViewModel) model;
                languageModel?.ChangeLanguage(l);
            });
            await ProgressBarUtilities.AnimateProgressBar(Progress, 101, 10, this, x => x.Progress);
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
            await ProgressBarUtilities.AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);
            ProzessStatus = "";
            return await FillUpdateTableNormal();
        }

        private async Task<UpdateRestoreStatus> FillUpdateTableNormal()
        {
            var result = await AddDownloadFilesToRestoreTable(VersionUtilities.GetLatestModVersion(), null);
            if (result != UpdateRestoreStatus.Succeeded)
            {
                if (result == UpdateRestoreStatus.Error)
                    MessageProvider.Show(MessageProvider.GetMessage("ExceptionUpdateVersionNotMatch"));
                return result;
            }
            return await AddDeleteFilesToRestoreTable(false);
        }

        private async Task<UpdateRestoreStatus> PrepareVoiceIgnoreUpdate()
        {
            await ProgressBarUtilities.AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);
            ProzessStatus = MessageProvider.GetMessage("UpdateStatusPrepare");
            return await FillUpdateTableVoice();
        }

        private async Task<UpdateRestoreStatus> FillUpdateTableVoice()
        {
            var result = await AddDownloadFilesToRestoreTable(VersionUtilities.GetLatestModVersion(), new List<string>
            {
                @"\Data\Audio\Speech\*",
                @"\Data\*Speech.meg",
                @"\Data\Audio\"
            });
            if (result != UpdateRestoreStatus.Succeeded)
            {
                if (result == UpdateRestoreStatus.Error)
                    MessageProvider.Show(MessageProvider.GetMessage("ExceptionRestoreVersionNotMatch"));
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
                MessageProvider.Show(MessageProvider.GetMessage("UpdateDone"));
            else if (result == UpdateRestoreStatus.Canceled)
                MessageProvider.Show(MessageProvider.GetMessage("UpdateAborted"));
            ResetUi();
        }

        protected override RestoreUpdateOperation ViewModelOperation => RestoreUpdateOperation.Update;

        protected override void ChangeSelection(object obj)
        {
            base.ChangeSelection(obj);
            SelectedOption = (UpdateOptions) obj;
        }

        public Command OpenChangelogCommand => new Command(OpenChangelog);

        private void OpenChangelog()
        {
            //await Task.Run(() => HostServer.DownloadFile("master\\" + ChangelogFileName,
            //    LauncherViewModel.RestoreDownloadDir + ChangelogFileName));
            //var process = new Process
            //{
            //    StartInfo = {FileName = LauncherViewModel.RestoreDownloadDir + ChangelogFileName}
            //};
            //process.Start();
            Process.Start(Configuration.Config.RaWChangelog);
        }
    }

    [Flags]
    public enum UpdateOptions
    {
        None = 1,
        IgnoreVoice = 2
    }
}