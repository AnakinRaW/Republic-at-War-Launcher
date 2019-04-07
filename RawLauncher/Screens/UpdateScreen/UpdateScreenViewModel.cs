using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Input.Command;
using RawLauncher.Framework.Launcher;
using RawLauncher.Framework.Mods;
using RawLauncher.Framework.Screens.LanguageScreen;
using RawLauncher.Framework.Server;
using RawLauncher.Framework.Shell;
using RawLauncher.Framework.Utilities;

namespace RawLauncher.Framework.Screens.UpdateScreen
{
    [Export(typeof(ILauncherScreen))]
    [Export(typeof(IUpdateScreen))]
    public class UpdateScreenViewModel : UpdateRestoreBase, IUpdateScreen
    {
        private int _count;

        private UpdateOptions SelectedOption { get; set; }

        protected override RestoreUpdateOperation ViewModelOperation => RestoreUpdateOperation.Update;

        public ICommand OpenChangelogCommand => new Command(OpenChangelog);

        public ICommand UpdateModCommand => new Command(UpdateMod);

        [ImportingConstructor]
        public UpdateScreenViewModel(LauncherModel launcher, IHostServer server) : base(launcher, server)
        {
        }

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

            ProcessStatus = MessageProvider.GetMessage("UpdateStatusPrepare");


            if (Launcher.CurrentMod == null)
                Launcher.CurrentMod = new DummyMod(Launcher.BaseGame);
            var l = Launcher.CurrentMod.InstalledLanguage;

            //if (LauncherViewModel.CurrentMod.Version <= Version.Parse("1.1.5"))
            //{
            Launcher.BaseGame.ClearDataFolder();
            Launcher.BaseGame.Patch();
            //}

            await ProgressBarUtilities.AnimateProgressBar(Progress, 10, 0, this, x => x.Progress);

            var getXmlResult = await GetXmlData(VersionUtilities.GetLatestModVersion());
            if (getXmlResult != LoadRestoreUpdateResult.Succeeded)
            {
                switch (getXmlResult)
                {
                    case LoadRestoreUpdateResult.Offline:
                        MessageProvider.Show(MessageProvider.GetMessage("UpdateHostOffline"));
                        break;
                    case LoadRestoreUpdateResult.WrongVersion:
                        MessageProvider.Show(MessageProvider.GetMessage("UpdateVersionNotFound"));
                        break;
                    case LoadRestoreUpdateResult.StreamEmpty:
                        MessageProvider.Show(MessageProvider.GetMessage("UpdateStreamNull"));
                        break;
                    case LoadRestoreUpdateResult.StreamBroken:
                        MessageProvider.Show(MessageProvider.GetMessage("UpdateXmlNotValid"));
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
                        result = await PrepareVoiceIgnoreUpdate();
                        if (result != UpdateRestoreStatus.Succeeded)
                        {
                            return result;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(SelectedOption));
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

            if (Launcher.CurrentMod is DummyMod)
                Launcher.CurrentMod = new RaW().FindMod(Launcher.BaseGame);

            IoC.Get<ILauncherMainWindow>().InstalledVersion = Launcher.CurrentMod.Version;

            ProcessStatus = "UpdateStatusFinishing";
            await Task.Run(() =>
            {
                IoC.Get<ILanguageScreen>()?.ChangeLanguage(l);
            });
            await ProgressBarUtilities.AnimateProgressBar(Progress, 101, 10, this, x => x.Progress);
            return UpdateRestoreStatus.Succeeded;
        }

        protected override void ResetUi()
        {
            base.ResetUi();
            if (!(Launcher.CurrentMod is DummyMod))
                return;
            IoC.Get<ILauncherMainWindow>().IsBlocked = true;
            CanExecute = true;
        }

        protected override bool AskUserToContinue()
        {
            return UpdateHelper.AskUserToContinue();
        }

        protected override void ChangeSelection(object obj)
        {
            base.ChangeSelection(obj);
            SelectedOption = (UpdateOptions)obj;
        }

        private async Task<UpdateRestoreStatus> PrepareNormalUpdate()
        {
            await ProgressBarUtilities.AnimateProgressBar(Progress, 0, 0, this, x => x.Progress);
            ProcessStatus = "";
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
            ProcessStatus = MessageProvider.GetMessage("UpdateStatusPrepare");
            return await FillUpdateTableVoice();
        }

        private async Task<UpdateRestoreStatus> FillUpdateTableVoice()
        {
            var result = await AddDownloadFilesToRestoreTable(VersionUtilities.GetLatestModVersion(), new List<string>
            {
                @"Data\Audio\Speech\*",
                @"Data\*Speech.meg",
                @"Data\Audio\",
                @"Data\Art\Movies\Binked\"
            });
            if (result != UpdateRestoreStatus.Succeeded)
            {
                if (result == UpdateRestoreStatus.Error)
                    MessageProvider.Show(MessageProvider.GetMessage("ExceptionRestoreVersionNotMatch"));
                return result;
            }
            return await AddDeleteFilesToRestoreTable(false);
        }

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

        private void OpenChangelog()
        {
            Process.Start(Configuration.Config.RaWChangelog);
        }
    }

    [Flags]
    internal enum UpdateOptions
    {
        None = 1,
        IgnoreVoice = 2
    }
}
