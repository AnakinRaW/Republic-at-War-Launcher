using System;
using System.Collections.Generic;
using System.Windows;

namespace RawLauncherWPF.Localization
{
    sealed class English : Language
    {
        public English()
        {
            StringTable = new Dictionary<string, string>();
            FillTable();
        }

        protected override Dictionary<string, string> StringTable { get; }

        protected override void FillTable()
        {
            AddCheckStrigns();
            AddErrorStrings();
            AddExceptionStrings();
            AddLanguageStrings();
            AddLauncherStrings();
            AddModStrings();
            AddPlayStrings();
            AddResourceExtractorStrings();
            AddRestoreStrigns();
            AddUpdateStrings(); AddVersionStrings();
        }

        public override void Reload()
        {
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("/RawLauncherWPF;component/Localization/English.xaml", UriKind.Relative)
            });
        }

        private void AddCheckStrigns()
        {
            StringTable.Add("CheckFolderNotValid", "{0}-Fail: {1}");
            StringTable.Add("CheckGamesNotPatchedMessage", "You need to update your games. Please press the 'patch' button.");
            StringTable.Add("CheckMessageAborted", "aborted");
            StringTable.Add("CheckMessageAiCorrect", "ai correct");
            StringTable.Add("CheckMessageAiWrong", "ai wrong");
            StringTable.Add("CheckMessageCouldNotCheck", "could not check");
            StringTable.Add("CheckMessageGameFound", "foc found");
            StringTable.Add("CheckMessageGameNotFound", "foc not found");
            StringTable.Add("CheckMessageGamesNotPatched", "games not patched");
            StringTable.Add("CheckMessageGamesPatched", "games patched");
            StringTable.Add("CheckMessageModCorrect", "mod correct");
            StringTable.Add("CheckMessageModFound", "raw found");
            StringTable.Add("CheckMessageModNotFound", "raw not found");
            StringTable.Add("CheckMessageModWrong", "mod wrong");
            StringTable.Add("CheckOfflineXmlNotFound", "Could not find the necessary files to check your version. It was also not possible to check them with our server. Please click Restore-Tab and let the launcher redownload the Files.");
            StringTable.Add("CheckPatchGamesFailed", "Games not successfuly patched.");
            StringTable.Add("CheckPatchGamesFailedBase", "BaseGame not successfuly patched. Eaw successfuly patched");
            StringTable.Add("CheckPatchGamesFailedEaw", "BaseGame successfuly patched. Eaw not successfuly patched.");
            StringTable.Add("CheckPatchGamesSuccess", "Games successfuly patched.");
            StringTable.Add("CheckStatusChecking", "Checking: {0}");
            StringTable.Add("CheckStatusCheckingGameExist", "Checking Game existance");
            StringTable.Add("CheckStatusCheckingGamePatches", "Checking Game Patches");
            StringTable.Add("CheckStatusCheckingModExist", "Checking Mod existance");
            StringTable.Add("CheckStatusPrepareAiModCheck", "Preparing AI/Mod Check");
            StringTable.Add("CheckVersionNotFound", "Your installed version is not available to check. Please try later or contact us.");
            StringTable.Add("CheckXmlValidateError", "The necessary files are not valid. It was also not possible to check them with our server. Please click Restore-Tab and let the launcher redownload the Files.");
            StringTable.Add("CheckXmlWrongVersion", "The Version of the mod does not match to the reference file. Please click Restore-Tab and let the launcher redownload the Files.");
        }

        private void AddErrorStrings()
        {
            StringTable.Add("ErrorAlreadyRunning", "One instance of the launcher is already running.");
            StringTable.Add("ErrorCreateMessageFailed", "Was unable to create a text");
            StringTable.Add("ErrorInitLauncher", "Something went wrong when initializing the Launcher: {0}");
            StringTable.Add("ErrorInitFailed", "The launcher was not able to find an installation of the games EaW/FoC or the Mod. Please run the launcher from FoC-Root Directory");
        }

        private void AddExceptionStrings()
        {
            StringTable.Add("ExceptionGameExist", "This Game does not exists");
            StringTable.Add("ExceptionGameExistName", "{0} does not exists.");
            StringTable.Add("ExceptionGameModCompatible", "Mod is not compatible");
            StringTable.Add("ExceptionGameModExist", "Mod does not exists anymore.");
            StringTable.Add("ExceptionGameModWrongInstalled", "Mod is not correct installed");
            StringTable.Add("ExceptionHostServerGetData", "Was not able to get data from: {0}");
            StringTable.Add("ExceptionModExist", "This Mod does not exist");
            StringTable.Add("ExceptionModExistName", "{0} does not exist");
            StringTable.Add("ExceptionResourceExtractorNotFound", "The Resource could not be found: {0}");
            StringTable.Add("ExceptionRestoreVersionNotMatch", "Versions do not match");
            StringTable.Add("ExceptionUpdateVersionNotMatch", "Versions do not match");
            StringTable.Add("ExceptionXmlParserError", "Unable to deserialize the xml stream. {0}");
            StringTable.Add("ExceptionSteamClientMissing", "Was unable to find the Steam Client-App");
        }

        private void AddLanguageStrings()
        {
            StringTable.Add("LanguageAdditionalSupport", "There is also a separate specific Version available for this language. Please check Moddb.com for a Language-Pack or check the RaW installer for more options.");
            StringTable.Add("LanguageMessageChangedSuccess", "Successfuly Changed Language");
            StringTable.Add("LanguageMessageSpeechFileRenameFailed", "Could not change Speech.meg");
            StringTable.Add("LanguageMessageSpeechRenameFailed", "Could not change Speech folder");
            StringTable.Add("LanguageMessageTextRenameFailed", "Could not change MasterTextFile");
            StringTable.Add("LanguageNoneSelected", "Select a language, you want to use.");
            StringTable.Add("LangugeOperationQuestion", "Note that chaging the language in this case means that any [MISSING]s and missing audio will be replaced with the default english version. For any language packs consider our Moddb page or check the Republic at War installer for language options");
        }

        private void AddLauncherStrings()
        {
            StringTable.Add("LauncherInfoNewVersion", "New Version: {0} is avaiable.");
        }

        private void AddModStrings()
        {
            StringTable.Add("ModVersionNotFound", "Could not get the current version. Please reinstall the Republic at War and try again.");
        }

        private void AddPlayStrings()
        {
            StringTable.Add("PlayCurrentSessionWait", "wait..");
        }

        private void AddResourceExtractorStrings()
        {
            StringTable.Add("ResourceExtractorNewDirectoryCreated", "Created new Directory: {0}");
        }

        private void AddRestoreStrigns()
        {
            StringTable.Add("RestoreAborted", "Restoring aborted");
            StringTable.Add("RestoreDone", "Restoring Done");
            StringTable.Add("RestoreErrorExit", "Either you aborted the Progress of something failed");
            StringTable.Add("RestoreHostServerOffline", "Could not Download the required files, because the servers are offline. Please try later.");
            StringTable.Add("RestoreInternetLost", "You lost your Internet connection. In order to prevent much more error messages the progress will be cancelled now.");
            StringTable.Add("RestoreNoInternet", "You need an Internet connction to Restore your mod");
            StringTable.Add("RestoreNoVersion", "You need to select a Version to restore to.");
            StringTable.Add("RestoreOperationQuestion", "Are you sure you want to restore Republic at War ? This cannot be undone. Modified Files will be delted and restored with the original ones.");
            StringTable.Add("RestoreStatusCheckAdditionalFiles", "Checking for additional files");
            StringTable.Add("RestoreStatusCheckMissing", "Checking for missing/corrupted files");
            StringTable.Add("RestoreStatusDownloaded", "Downloaded: {0}");
            StringTable.Add("RestoreStatusFinishing", "Finishing");
            StringTable.Add("RestoreStatusPrepare", "Preparing Restore");
            StringTable.Add("RestoreStatusPrepareTable", "Preparing download-table");
            StringTable.Add("RestoreStautsDeletingMod", "Deleting Mod Files");
            StringTable.Add("RestoreStreamNull", "Error while downloading the required files. Please try later");
            StringTable.Add("RestoreTableNull", "Error while trying to download the mod files.  The required Table was empty. Please try again");
            StringTable.Add("RestoreVersionNotMatch", "Your installed version is not available to check. Please try later or contact us.");
            StringTable.Add("RestoreXmlNotValid", "The necessary files are not valid. It was also not possible to check them with our server. Please click Restore-Tab and let the launcher redownload the Files.");
        }

        private void AddUpdateStrings()
        {
            StringTable.Add("UpdateAborted", "Update aborted");
            StringTable.Add("UpdateDone", "Updateing Done");
            StringTable.Add("UpdateErrorExit", "Either you aborted the Progress of something failed");
            StringTable.Add("UpdateHostOffline", "Could not Download the required files, because the servers are offline.  Please try later");
            StringTable.Add("UpdateInternetLost", "You lost your Internet connection. In order to prevent much more error messages the progress will be cancelled now.");
            StringTable.Add("UpdateNoInternet", "You need an Internet connction to update your mod");
            StringTable.Add("UpdateOperationQuestion", "Are you sure you want to update Republic at War ? This cannot be undone. Modified Files will also be delted and restored with the original ones.");
            StringTable.Add("UpdateStatusCheckAdditionalFiles", "Checking for additional files");
            StringTable.Add("UpdateStatusCheckNew", "Checking for new and corruppted files");
            StringTable.Add("UpdateStatusDownloaded", "Downloaded: {0}");
            StringTable.Add("UpdateStatusFinishing", "Finishing");
            StringTable.Add("UpdateStatusPrepare", "Preparing Update");
            StringTable.Add("UpdateStreamNull", "Error while downloading the required files. Please try later");
            StringTable.Add("UpdateTableNull", "Error while trying to download the mod files. The required Table was empty. Please try again");
            StringTable.Add("UpdateVersionNotFound", "Your installed version is not available to check. Please try later or contact us.");
            StringTable.Add("UpdateXmlNotValid", "The necessary files are not valid. It was also not possible to check them with our server. Please click Restore-Tab and let the launcher redownload the Files.");
        }

        private void AddVersionStrings()
        {
            StringTable.Add("VersionUtilitiesAskForUpdate", "New Version: {0} avaiable. Update now ? ");
        }
    }
}
