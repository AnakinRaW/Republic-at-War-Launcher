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
            StringTable.Add("CheckGamesNotPatchedMessage", "Forces of Corruption is not up to date. Please press 'patch' to update your game.");
            StringTable.Add("CheckMessageAborted", "aborted");
            StringTable.Add("CheckMessageAiCorrect", "ai configured");
            StringTable.Add("CheckMessageAiWrong", "ai misconfigured");
            StringTable.Add("CheckMessageCouldNotCheck", "could not check");
            StringTable.Add("CheckMessageGameFound", "foc found");
            StringTable.Add("CheckMessageGameNotFound", "foc not found");
            StringTable.Add("CheckMessageGamesNotPatched", "game not patched");
            StringTable.Add("CheckMessageGamesPatched", "game patched");
            StringTable.Add("CheckMessageModCorrect", "mod configured");
            StringTable.Add("CheckMessageModFound", "raw found");
            StringTable.Add("CheckMessageModNotFound", "raw not found");
            StringTable.Add("CheckMessageModWrong", "raw misconfigured");
            StringTable.Add("CheckOfflineXmlNotFound", "Version check failed. Please click the 'Restore' tab and restore the mod files.");
            StringTable.Add("CheckPatchGamesFailed", "Game patch failed.");
            StringTable.Add("CheckPatchGamesFailedBase", "Forces of Corruption patch failed. Empire at War patched successfully.");
            StringTable.Add("CheckPatchGamesFailedEaw", "Forces of Corruption patch failed. Empire at War patched successfully.");
            StringTable.Add("CheckPatchGamesSuccess", "Game patched successfully.");
            StringTable.Add("CheckStatusChecking", "Checking: {0}");
            StringTable.Add("CheckStatusCheckingGameExist", "Searching for FoC");
            StringTable.Add("CheckStatusCheckingGamePatches", "Checking game patches");
            StringTable.Add("CheckStatusCheckingModExist", "Searching for RaW");
            StringTable.Add("CheckStatusPrepareAiModCheck", "Preparing AI/mod check");
            StringTable.Add("CheckVersionNotFound", "Unable to retrieve the specified version from the remote servers. Please try again later.");
            StringTable.Add("CheckXmlValidateError", "The necessary XML files are not valid. Please click the 'Restore' tab and restore the mod files.");
            StringTable.Add("CheckXmlWrongVersion", "This version of the mod does not match to the reference file. Please click the 'Restore' tab and restore the mod files.");
        }

        private void AddErrorStrings()
        {
            StringTable.Add("ErrorAlreadyRunning", "Another instance of the launcher is already running.");
            StringTable.Add("ErrorCreateMessageFailed", "Unable to create message text.");
            StringTable.Add("ErrorInitLauncher", "There was a problem initializing the launcher: {0}");
            StringTable.Add("ErrorInitFailed", "The launcher was not able to find Forces of Corruption. Please run the launcher from root Forces of Corruption directory.");
        }

        private void AddExceptionStrings()
        {
            StringTable.Add("ExceptionGameExist", "Game could not be found.");
            StringTable.Add("ExceptionGameExistName", "{0} could not be found.");
            StringTable.Add("ExceptionGameModCompatible", "Mod is not compatible");
            StringTable.Add("ExceptionGameModExist", "Mod could not be found.");
            StringTable.Add("ExceptionGameModWrongInstalled", "Mod is not installed.");
            StringTable.Add("ExceptionHostServerGetData", "Was not able to get data from: {0}");
            StringTable.Add("ExceptionModExist", "Mod could not be found.");
            StringTable.Add("ExceptionModExistName", "{0} could not be found.");
            StringTable.Add("ExceptionResourceExtractorNotFound", "The resource could not be found: {0}");
            StringTable.Add("ExceptionRestoreVersionNotMatch", "Versions do not match");
            StringTable.Add("ExceptionUpdateVersionNotMatch", "Versions do not match");
            StringTable.Add("ExceptionXmlParserError", "Unable to deserialize the xml stream. {0}");
            StringTable.Add("ExceptionSteamClientMissing", "Unable to find the Steam client app");
        }

        private void AddLanguageStrings()
        {
            StringTable.Add("LanguageAdditionalSupport", "There is a separate specific version available for this language. Please check Moddb.com for a Language-Pack or check the RaW installer for more options.");
            StringTable.Add("LanguageMessageChangedSuccess", "Language changed successfully.");
            StringTable.Add("LanguageMessageSpeechFileRenameFailed", "Could not update Speech.meg.");
            StringTable.Add("LanguageMessageSpeechRenameFailed", "Could not update speech folder.");
            StringTable.Add("LanguageMessageTextRenameFailed", "Could not update MasterTextFile.");
            StringTable.Add("LanguageNoneSelected", "Please select a language.");
            StringTable.Add("LangugeOperationQuestion", "Note that changing the language means that any [MISSING]s and missing audio will be replaced with the default english version. For any other language packs, consider our Moddb page or check the Republic at War installer for language options.");
        }

        private void AddLauncherStrings()
        {
            StringTable.Add("LauncherInfoNewVersion", "New version: {0} is avaiable.");
        }

        private void AddModStrings()
        {
            StringTable.Add("ModVersionNotFound", "Could not find the current version. Please reinstall the Republic at War and try again.");
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
            StringTable.Add("RestoreDone", "Restoring done");
            StringTable.Add("RestoreErrorExit", "Process aborted or failed.");
            StringTable.Add("RestoreHostServerOffline", "Unable to download the required files as the servers could not be reached. Please try later.");
            StringTable.Add("RestoreInternetLost", "You lost your internet connection. In order to prevent further error messages, the progress will be cancelled now.");
            StringTable.Add("RestoreNoInternet", "You need an internet connection to execute a restore.");
            StringTable.Add("RestoreNoVersion", "Please select a version to restore to.");
            StringTable.Add("RestoreOperationQuestion", "Are you sure you want to restore Republic at War? This cannot be undone. Modified files will be deleted and restored to their default state.");
            StringTable.Add("RestoreStatusCheckAdditionalFiles", "Checking for additional files.");
            StringTable.Add("RestoreStatusCheckMissing", "Checking for missing/corrupted files.");
            StringTable.Add("RestoreStatusDownloaded", "Downloaded: {0}.");
            StringTable.Add("RestoreStatusFinishing", "Finishing.");
            StringTable.Add("RestoreStatusPrepare", "Preparing restore.");
            StringTable.Add("RestoreStatusPrepareTable", "Preparing download table.");
            StringTable.Add("RestoreStautsDeletingMod", "Deleting mod files.");
            StringTable.Add("RestoreStreamNull", "Error while downloading the required files. Please try later.");
            StringTable.Add("RestoreTableNull", "Error while trying to download the mod files. The required table was empty. Please try again.");
            StringTable.Add("RestoreVersionNotMatch", "Unable to retrieve the specified version from the remote servers. Please try later.");
            StringTable.Add("RestoreXmlNotValid", "The necessary XML files are not valid. Please click the 'Restore' tab and restore the mod files.");
        }

        private void AddUpdateStrings()
        {
            StringTable.Add("UpdateAborted", "Update aborted.");
            StringTable.Add("UpdateDone", "Updateing done.");
            StringTable.Add("UpdateErrorExit", "Process aborted or failed.");
            StringTable.Add("UpdateHostOffline", "Unable to download the required files as the servers could not be reached. Please try later.");
            StringTable.Add("UpdateInternetLost", "You lost your internet connection. In order to prevent further error messages, the progress will be cancelled now.");
            StringTable.Add("UpdateNoInternet", "You need an internet connction to update the Republic at War.");
            StringTable.Add("UpdateOperationQuestion", "Are you sure you want to update Republic at War? This cannot be undone. Modified files will be deleted and restored to their default state.");
            StringTable.Add("UpdateStatusCheckAdditionalFiles", "Checking for additional files");
            StringTable.Add("UpdateStatusCheckNew", "Checking for new and corruppted files");
            StringTable.Add("UpdateStatusDownloaded", "Downloaded: {0}.");
            StringTable.Add("UpdateStatusFinishing", "Finishing.");
            StringTable.Add("UpdateStatusPrepare", "Preparing update.");
            StringTable.Add("UpdateStreamNull", "Error while downloading the required files. Please try later.");
            StringTable.Add("UpdateTableNull", "Error while trying to download the mod files. The required table was empty. Please try again.");
            StringTable.Add("UpdateVersionNotFound", "Unable to retrieve the specified version from the remote servers. Please try later.");
            StringTable.Add("UpdateXmlNotValid", "The necessary XML files are not valid. Please click the 'Restore' tab and restore the mod files.");
        }

        private void AddVersionStrings()
        {
            StringTable.Add("VersionUtilitiesAskForUpdate", "New Version: {0} avaiable. Update now?");
        }
    }
}
