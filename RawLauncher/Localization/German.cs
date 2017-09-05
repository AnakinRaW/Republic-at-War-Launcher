using System;
using System.Collections.Generic;
using System.Windows;

namespace RawLauncher.Framework.Localization
{
    public sealed class German : Language 
    {
        public German()
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
            AddUpdateStrings();
            AddVersionStrings();
        }

        public override void Reload()
        {
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("/RawLauncher.Framework;component/Localization/German.xaml", UriKind.Relative)
            });
        }

        private void AddCheckStrigns()
        {
            StringTable.Add("CheckFolderNotValid", "Verzeichnis fehlerhaft");
            StringTable.Add("CheckAIFolderNotValid", "Das Data Verzeichnis des Spiels stimmt nicht mit deiner installierten Verion von Republic at War überein.\r\n\r\n"
                                                     + "Möchtest du dir den Fehlerbericht jetzt anzeigen lassen?");
            StringTable.Add("CheckModFolderNotValid", "Das Mod Verzeichnis von Republic at War stimmt nicht mit deiner installierten Verion von Republic at War überein.\r\n\r\n"
                                                     + "Möchtest du dir den Fehlerbericht jetzt anzeigen lassen?");
            StringTable.Add("CheckFolderNotValidCount", "Das folgende Verzeichnis besitzt die falsche Anzahl an Dateien: {0}");
            StringTable.Add("CheckFolderNotValidExists", "Das folgende Verzeichnis exisitert nicht: {0}");
            StringTable.Add("CheckFolderNotValidHash", "Das folgende Verzeichnis besitzt fehlerhafte Dateien: {0}");
            StringTable.Add("CheckGamesNotPatchedMessage", "Du musst die Spiele updaten. Bitte drücke den 'Patch'-Button");
            StringTable.Add("CheckMessageAborted", "abgebrochen");
            StringTable.Add("CheckMessageAiCorrect", "ai richtig");
            StringTable.Add("CheckMessageAiWrong", "ai flasch");
            StringTable.Add("CheckMessageCouldNotCheck", "konnte nicht prüfen");
            StringTable.Add("CheckMessageGameFound", "foc gefunden");
            StringTable.Add("CheckMessageGameNotFound", "foc nicht gefunden");
            StringTable.Add("CheckMessageGamesNotPatched", "spiele nicht geupdated");
            StringTable.Add("CheckMessageGamesPatched", "spile geupdated");
            StringTable.Add("CheckMessageModCorrect", "mod richtig");
            StringTable.Add("CheckMessageModFound", "raw gefunden");
            StringTable.Add("CheckMessageModNotFound", "raw nicht gefunden");
            StringTable.Add("CheckMessageModWrong", "mod falsch");
            StringTable.Add("CheckOfflineXmlNotFound", "Konnte die erforderlichen Dateien zum überprüfen der Version nicht finden. Ebenfalls konnten die Dateien nicht auf deiner Festplatte gefunden werden. Bitte setze den Mod zurück, um die nötigen Dateien nachzuladen.");
            StringTable.Add("CheckPatchGamesFailed", "Die Spiele konnten nicht gepatcht werden");
            StringTable.Add("CheckPatchGamesFailedBase", "Das Hauptspiel konnte nicht gepatcht werden. EaW wurde erfolgreich gepatcht");
            StringTable.Add("CheckPatchGamesFailedEaw", "Das Hauptspiel wurde erfolgreich gepacht. EaW konnte nicht gepatcht werden.");
            StringTable.Add("CheckPatchGamesSuccess", "Spiele erfolgreich gepatcht");
            StringTable.Add("CheckStatusChecking", "Überprüfe: {0}");
            StringTable.Add("CheckStatusCheckingGameExist", "Prüfe Existenz des Hauptspiel");
            StringTable.Add("CheckStatusCheckingGamePatches", "Prüfe die Spiele nach Version");
            StringTable.Add("CheckStatusCheckingModExist", "Prüfe Existenz von Republic at War");
            StringTable.Add("CheckStatusPrepareAiModCheck", "Bereit den AI- und Mod-Check vor");
            StringTable.Add("CheckVersionNotFound", "Deine aktuell installierte Mod-Version konnte nicht geprüft werden. Bitte versuche es später erneut.");
            StringTable.Add("CheckXmlValidateError", "Die relevanten Dateien waren ungültig oder fehlerhaft. Ebenfalls konnten die Dateien nicht auf deiner Festplatte gefunden werden. Bitte setze den Mod zurück, um die nötigen Dateien nachzuladen.");
            StringTable.Add("CheckXmlWrongVersion", "Die aktuelle Version des Mod entspricht nicht dem der Prüfdatei. Bitte setze den Mod zurück, um die nötigen Dateien nachzuladen.");
        }

        private void AddErrorStrings()
        {
            StringTable.Add("ErrorAlreadyRunning", "Es läuft bereits eine Instanz des Launchers.");
            StringTable.Add("ErrorCreateMessageFailed", "Es konnte kein Text erstellt werden");
            StringTable.Add("ErrorInitLauncher", "Fehler beim initialisieren dem Launcher { 0}");
            StringTable.Add("ErrorInitFailed", "Der Launcher konnte die Spiele EaW oder Foc nicht finden. Bitte starte den Launcher im Hauptverzeichnis von FoC.");
            StringTable.Add("ErrorInitFailedMod", "Der Launcher konnte Republic at War nicht finden. Klick auf 'Update' im Update-Tab und installiere Republic at War jetzt.\r\n" +
                                                  "\r\nSolltest das Hauptspiel die Steam-Version sein, sollte Republic at War zunächst über den Steam Workshops heruntergeladen werden.");
        }

        private void AddExceptionStrings()
        {
            StringTable.Add("ExceptionGameExist", "Das Spiel existiert nicht");
            StringTable.Add("ExceptionGameExistName", "{0} existiert nicht.");
            StringTable.Add("ExceptionGameModCompatible", "Der Mod ist nicht kompatibel");
            StringTable.Add("ExceptionGameModExist", "Der Mod existiert nicht");
            StringTable.Add("ExceptionGameModWrongInstalled", "Der Mod ist nicht korrekt installiert");
            StringTable.Add("ExceptionHostServerGetData", "Konnte folgende Daten nicht runterladen: {0}");
            StringTable.Add("ExceptionModExist", "Der Mod existiert nicht");
            StringTable.Add("ExceptionModExistName", "{0} existiert nicht");
            StringTable.Add("ExceptionResourceExtractorNotFound", "Folgende Ressource konnte nicht gefunden werden: {0}");
            StringTable.Add("ExceptionRestoreVersionNotMatch", "Die Versionen stimmen nicht überein");
            StringTable.Add("ExceptionUpdateVersionNotMatch", "Die Versionen stimmen nicht überein");
            StringTable.Add("ExceptionXmlParserError", "Konnte den Datenstrom nicht umwandeln. {0}");
            StringTable.Add("ExceptionSteamClientMissing", "Konnte den Steam-Client nicht finden.");
            StringTable.Add("DownloadFailed", "Downloads fehlgeschlagen");
            StringTable.Add("DownloadFailedQuestion", "Der Download einiger Dateien ist fehlgeschlagen. Versuche es bitte erneut.\r\n\r\n" +
                                              "Möchtest du dir den Fehlerbericht jetzt anzeigen lassen?");
        }

        private void AddLanguageStrings()
        {
            StringTable.Add("LanguageAdditionalSupport", "Für diese Sprache existiert auch ein erweitertes Sprachpaket. Please check. Du kannst es dir auf Moddb.com herunterladen oder beim Installer auswählen.");
            StringTable.Add("LanguageMessageChangedSuccess", "Sprache wurde erfolgreich geändert.");
            StringTable.Add("LanguageMessageSpeechFileRenameFailed", "Konnte die 'Speech.meg'-Datei nicht umbenennen.");
            StringTable.Add("LanguageMessageSpeechRenameFailed", "Konnte den Ordner im Speech-Verzeichnis nicht umbenennen.");
            StringTable.Add("LanguageMessageTextRenameFailed", "Konnte die MasterTextFile nicht umbenennen.");
            StringTable.Add("LanguageNoneSelected", "Bitte wähle eine Sprache aus.");
            StringTable.Add("LangugeOperationQuestion", "Bitte beachte, dass das ändern der Sprache in diesem Fall lediglich bedeutet, dass [Missings] durch den Originaltext ersetzt werden. Für erweiterte Sprachpakete besuche bitte die Moddb-Seite von Republic at War");
        }

        private void AddLauncherStrings()
        {
            StringTable.Add("LauncherInfoNewVersion", "Neue Version: {0} ist verfügbar");
        }

        private void AddModStrings()
        {
            StringTable.Add("ModVersionNotFound", "Die aktuelle Version von Republic at War konnte nicht festegestellt werden. Bitte lade Republic at War neu herunter.");
            StringTable.Add("UninstallModWarning", "Bist du dir sicher, dass du Republic at War löschen willst. Dieser Vorgang kann nicht rückgängig gemacht werden.");
        }

        private void AddPlayStrings()
        {
            StringTable.Add("PlayCurrentSessionWait", "warte..");
        }

        private void AddResourceExtractorStrings()
        {
            StringTable.Add("ResourceExtractorNewDirectoryCreated", "Es wurde ein neues Verzeichnis erstellt: {0}");
        }

        private void AddRestoreStrigns()
        {
            StringTable.Add("RestoreAborted", "Wiederherstellung abgebrochen");
            StringTable.Add("RestoreDone", "Wiederherstellung erfolgreich");
            StringTable.Add("RestoreErrorExit", "Entwerder du hast den Vorgang beendet, oder es ist ein Fehler aufgetreten");
            StringTable.Add("RestoreHostServerOffline", "Konnte die relevanten Daten nicht runterladen, weil der Server offline ist. Bitte versuche es später erneut.");
            StringTable.Add("RestoreInternetLost", "Du hast deine Internetverbindung verloren. Um weitaus mehr Fehlermeldungen zu vermeiden wurde der Prozess jetzt beendet.");
            StringTable.Add("RestoreNoInternet", "Du braucht eine Internetverbindung, um dem Mod wiederherstellen zu können.");
            StringTable.Add("RestoreNoVersion", "Du musst eine Version auswählen, auf die du zurücksetzen willst.");
            StringTable.Add("RestoreOperationQuestion", "Bist du dir sicher, dass du Republic at War zurücksetzen willst. Dieser Vorgang kann nicht rückgängig gemacht werden. Alle von dir modfizierten Dateien werden überspielt.");
            StringTable.Add("RestoreStatusCheckAdditionalFiles", "Prüfe auf zusätzliche Dateien");
            StringTable.Add("RestoreStatusCheckMissing", "Prüfe auf modifizierte oder fehlende Dateien");
            StringTable.Add("RestoreStatusDownloaded", "Heruntergeladen: {0}");
            StringTable.Add("RestoreStatusFinishing", "Fertigstellen");
            StringTable.Add("RestoreStatusPrepare", "Wiederherstellung vorbereiten");
            StringTable.Add("RestoreStatusPrepareTable", "Erzeugen der Downloadtabelle");
            StringTable.Add("RestoreStautsDeletingMod", "Lösche Moddateien");
            StringTable.Add("RestoreStreamNull", "Fehler beim Runterladen von der Restoredatei. Bitte versuche es später erneut.");
            StringTable.Add("RestoreTableNull", "Fehler beim Runterladen von der Restoredatei. Die Downloadtabelle ist nicht gültig. Bitte versuche es später erneut.");
            StringTable.Add("RestoreVersionNotMatch", "Deine aktuell installierte Version ist nicht zum Wiederherstellen verfügbar. Bitte versuche es später erneut.");
            StringTable.Add("RestoreXmlNotValid", "Die relevanten Dateien konnten nicht heruntergeladen werden. Ebenfalls konnten die Dateien nicht auf deiner Festplatte gefunden werden. Bitte versuche es erneut.");
        }

        private void AddUpdateStrings()
        {
            StringTable.Add("UpdateAborted", "Update abgebrochen");
            StringTable.Add("UpdateDone", "Update abgeschlossen");
            StringTable.Add("UpdateErrorExit", "Entwerder du hast den Vorgang beendet, oder es ist ein Fehler aufgetreten");
            StringTable.Add("UpdateHostOffline", "Konnte die relevanten Daten nicht runterladen, weil der Server offline ist. Bitte versuche es später erneut.");
            StringTable.Add("UpdateInternetLost", "Du hast deine Internetverbindung verloren. Um weitaus mehr Fehlermeldungen zu vermeiden wurde der Prozess jetzt beendet.");
            StringTable.Add("UpdateNoInternet", "Du braucht eine Internetverbindung, um dem Mod updaten zu können.");
            StringTable.Add("UpdateOperationQuestion", "Bist du dir sicher, dass du Republic at War updaten willst ? Dieser Vorgang kann nicht rückgängig gemacht werden. Alle von dir modfizierten Dateien werden überspielt.");
            StringTable.Add("UpdateStatusCheckAdditionalFiles", "Prüfe auf zusätzliche Dateien");
            StringTable.Add("UpdateStatusCheckNew", "Prüfe auf neue oder fehlende Dateien");
            StringTable.Add("UpdateStatusDownloaded", "Heruntergeladen: {0}");
            StringTable.Add("UpdateStatusFinishing", "Abgeschlossen");
            StringTable.Add("UpdateStatusPrepare", "Update vorbereiten");
            StringTable.Add("UpdateStreamNull", "Fehler beim Runterladen von der Restoredatei. Bitte versuche es später erneut.");
            StringTable.Add("UpdateTableNull", "Fehler beim Runterladen von der Restoredatei. Die Downloadtabelle ist nicht gültig. Bitte versuche es später erneut.");
            StringTable.Add("UpdateVersionNotFound", "Deine aktuell installierte Version ist nicht zum Updaten verfügbar. Bitte versuche es später erneut.");
            StringTable.Add("UpdateXmlNotValid", "Die relevanten Dateien konnten nicht heruntergeladen werden. Ebenfalls konnten die Dateien nicht auf deiner Festplatte gefunden werden. Bitte versuche es erneut.");
        }

        private void AddVersionStrings()
        {
            StringTable.Add("VersionUtilitiesAskForUpdate", "Neue Version: {0} verfügbar. Jetzt herunterladen ? ");
        }
    }
}
