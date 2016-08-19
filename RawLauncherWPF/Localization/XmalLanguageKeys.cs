using System.Windows;

namespace RawLauncherWPF.Localization
{
    internal static class XmalLanguageKeys
    {
        private static ComponentResourceKey _buttonAbout;
        private static ComponentResourceKey _buttonUninstall;
        private static ComponentResourceKey _buttonPlay;
        private static ComponentResourceKey _buttonDefreeze;
        private static ComponentResourceKey _buttonOrganize;
        private static ComponentResourceKey _buttonPatch;
        private static ComponentResourceKey _buttonCheck;
        private static ComponentResourceKey _buttonLanguage;
        private static ComponentResourceKey _buttonRestore;
        private static ComponentResourceKey _buttonUpdate;
        private static ComponentResourceKey _buttonChangelog;
        private static ComponentResourceKey _buttonCancel;
        private static ComponentResourceKey _buttonClose;

        private static ComponentResourceKey _langDutch;
        private static ComponentResourceKey _langEnglish;
        private static ComponentResourceKey _langFrench;
        private static ComponentResourceKey _langGerman;
        private static ComponentResourceKey _langItalian;
        private static ComponentResourceKey _langRussian;
        private static ComponentResourceKey _langSerbian;
        private static ComponentResourceKey _langSpanish;
        private static ComponentResourceKey _langSwedish;
        private static ComponentResourceKey _langUkrainian;

        private static ComponentResourceKey _tabPlay;
        private static ComponentResourceKey _tabCheck;
        private static ComponentResourceKey _tabLanguage;
        private static ComponentResourceKey _tabRestore;
        private static ComponentResourceKey _tabUpdate;

        private static ComponentResourceKey _restoreNone;
        private static ComponentResourceKey _restoreLanguage;
        private static ComponentResourceKey _restoreHard;    

        private static ComponentResourceKey _updateNone;
        private static ComponentResourceKey _updateVoice;      

        private static ComponentResourceKey _textPatch;
        private static ComponentResourceKey _textLanguage;
        private static ComponentResourceKey _textMainInstalledVersion;
        private static ComponentResourceKey _textMainLatestVersion;
        private static ComponentResourceKey _textPlayCurrentGames;
        private static ComponentResourceKey _textRestoreVersion;
        private static ComponentResourceKey _textRestoreInfo;
        private static ComponentResourceKey _textDoNotShowAgain;

        private static ComponentResourceKey _toolNone;
        private static ComponentResourceKey _toolHard;
        private static ComponentResourceKey _toolLanguage;
        private static ComponentResourceKey _toolVoice;

        #region ToolTip
        public static ComponentResourceKey ToolNone => _toolNone ??
                                                           (_toolNone = new ComponentResourceKey(typeof(XmalLanguageKeys), "ToolNone"));

        public static ComponentResourceKey ToolHard => _toolHard ??
                                                           (_toolHard = new ComponentResourceKey(typeof(XmalLanguageKeys), "ToolHard"));

        public static ComponentResourceKey ToolLanguage => _toolLanguage ??
                                                           (_toolLanguage = new ComponentResourceKey(typeof(XmalLanguageKeys), "ToolLanguage"));

        public static ComponentResourceKey ToolVoice => _toolVoice ??
                                                           (_toolVoice = new ComponentResourceKey(typeof(XmalLanguageKeys), "ToolVoice"));
        #endregion

        #region Button
        public static ComponentResourceKey ButtonPlay => _buttonPlay ??
                                                            (_buttonPlay = new ComponentResourceKey(typeof(XmalLanguageKeys), "ButtonPlay"));

        public static ComponentResourceKey ButtonOrganize => _buttonOrganize ??
                                                            (_buttonOrganize = new ComponentResourceKey(typeof(XmalLanguageKeys), "ButtonOrganize"));

        public static ComponentResourceKey ButtonPatch => _buttonPatch ??
                                                            (_buttonPatch = new ComponentResourceKey(typeof(XmalLanguageKeys), "ButtonPatch"));

        public static ComponentResourceKey ButtonCheck => _buttonCheck ??
                                                            (_buttonCheck = new ComponentResourceKey(typeof(XmalLanguageKeys), "ButtonCheck"));

        public static ComponentResourceKey ButtonLanguage => _buttonLanguage ??
                                                            (_buttonLanguage = new ComponentResourceKey(typeof(XmalLanguageKeys), "ButtonLanguage"));

        public static ComponentResourceKey ButtonRestore => _buttonRestore ??
                                                            (_buttonRestore = new ComponentResourceKey(typeof(XmalLanguageKeys), "ButtonRestore"));

        public static ComponentResourceKey ButtonUpdate => _buttonUpdate ??
                                                            (_buttonUpdate = new ComponentResourceKey(typeof(XmalLanguageKeys), "ButtonUpdate"));

        public static ComponentResourceKey ButtonChangelog => _buttonChangelog ??
                                                            (_buttonChangelog = new ComponentResourceKey(typeof(XmalLanguageKeys), "ButtonChangelog"));

        public static ComponentResourceKey ButtonCancel => _buttonCancel ??
                                                            (_buttonCancel = new ComponentResourceKey(typeof(XmalLanguageKeys), "ButtonCancel"));

        public static ComponentResourceKey ButtonAbout => _buttonAbout ??
                                                    (_buttonAbout = new ComponentResourceKey(typeof(XmalLanguageKeys), "ButtonAbout"));

        public static ComponentResourceKey ButtonUninstall => _buttonUninstall ??
                                                    (_buttonUninstall = new ComponentResourceKey(typeof(XmalLanguageKeys), "ButtonUninstall"));

        public static ComponentResourceKey ButtonDefreeze => _buttonDefreeze ??
                                                            (_buttonDefreeze = new ComponentResourceKey(typeof(XmalLanguageKeys), "ButtonDefreeze"));

        public static ComponentResourceKey ButtonClose => _buttonClose ??
                                                            (_buttonClose = new ComponentResourceKey(typeof(XmalLanguageKeys), "ButtonClose"));
        #endregion

        #region Tabs
        public static ComponentResourceKey TabPlay => _tabPlay ??
                                                           (_tabPlay = new ComponentResourceKey(typeof(XmalLanguageKeys), "TabPlay"));

        public static ComponentResourceKey TabCheck => _tabCheck ??
                                                            (_tabCheck = new ComponentResourceKey(typeof(XmalLanguageKeys), "TabCheck"));

        public static ComponentResourceKey TabLanguage => _tabLanguage ??
                                                            (_tabLanguage = new ComponentResourceKey(typeof(XmalLanguageKeys), "TabLanguage"));

        public static ComponentResourceKey TabRestore => _tabRestore ??
                                                            (_tabRestore = new ComponentResourceKey(typeof(XmalLanguageKeys), "TabRestore"));

        public static ComponentResourceKey TabUpdate => _tabUpdate ??
                                                            (_tabUpdate = new ComponentResourceKey(typeof(XmalLanguageKeys), "TabUpdate"));
        #endregion

        #region Language
        public static ComponentResourceKey LangDutch => _langDutch ??
                                                            (_langDutch = new ComponentResourceKey(typeof(XmalLanguageKeys), "LangDutch"));

        public static ComponentResourceKey LangEnglish => _langEnglish ??
                                                            (_langEnglish = new ComponentResourceKey(typeof(XmalLanguageKeys), "LangEnglish"));

        public static ComponentResourceKey LangFrench => _langFrench ??
                                                            (_langFrench = new ComponentResourceKey(typeof(XmalLanguageKeys), "LangFrench"));

        public static ComponentResourceKey LangGerman => _langGerman ??
                                                            (_langGerman = new ComponentResourceKey(typeof(XmalLanguageKeys), "LangGerman"));

        public static ComponentResourceKey LangItalian => _langItalian ??
                                                            (_langItalian = new ComponentResourceKey(typeof(XmalLanguageKeys), "LangItalian"));

        public static ComponentResourceKey LangRussian => _langRussian ??
                                                            (_langRussian = new ComponentResourceKey(typeof(XmalLanguageKeys), "LangRussian"));

        public static ComponentResourceKey LangSerbian => _langSerbian ??
                                                            (_langSerbian = new ComponentResourceKey(typeof(XmalLanguageKeys), "LangSerbian"));

        public static ComponentResourceKey LangSpanish => _langSpanish ??
                                                            (_langSpanish = new ComponentResourceKey(typeof(XmalLanguageKeys), "LangSpanish"));

        public static ComponentResourceKey LangSwedish => _langSwedish ??
                                                            (_langSwedish = new ComponentResourceKey(typeof(XmalLanguageKeys), "LangSwedish"));

        public static ComponentResourceKey LangUkrainian => _langUkrainian ??
                                                            (_langUkrainian = new ComponentResourceKey(typeof(XmalLanguageKeys), "LangUkrainian"));
        #endregion

        #region Restore
        public static ComponentResourceKey RestoreNone => _restoreNone ??
                                                           (_restoreNone = new ComponentResourceKey(typeof(XmalLanguageKeys), "RestoreNone"));

        public static ComponentResourceKey RestoreLanguage => _restoreLanguage ??
                                                           (_restoreLanguage = new ComponentResourceKey(typeof(XmalLanguageKeys), "RestoreLanguage"));

        public static ComponentResourceKey RestoreHard => _restoreHard ??
                                                           (_restoreHard = new ComponentResourceKey(typeof(XmalLanguageKeys), "RestoreHard"));
        #endregion

        #region Update
        public static ComponentResourceKey UpdateNone => _updateNone ??
                                                           (_updateNone = new ComponentResourceKey(typeof(XmalLanguageKeys), "UpdateNone"));

        public static ComponentResourceKey UpdateVoice => _updateVoice ??
                                                           (_updateVoice = new ComponentResourceKey(typeof(XmalLanguageKeys), "UpdateVoice"));
        #endregion

        #region Text
        public static ComponentResourceKey TextPatch => _textPatch ??
                                                            (_textPatch = new ComponentResourceKey(typeof(XmalLanguageKeys), "TextPatch"));

        public static ComponentResourceKey TextLanguage => _textLanguage ??
                                                            (_textLanguage = new ComponentResourceKey(typeof(XmalLanguageKeys), "TextLanguage"));

        public static ComponentResourceKey TextMainInstalledVersion => _textMainInstalledVersion ??
                                                            (_textMainInstalledVersion = new ComponentResourceKey(typeof(XmalLanguageKeys), "TextMainInstalledVersion"));

        public static ComponentResourceKey TextMainLatestVersion => _textMainLatestVersion ??
                                                            (_textMainLatestVersion = new ComponentResourceKey(typeof(XmalLanguageKeys), "TextMainLatestVersion"));

        public static ComponentResourceKey TextPlayCurrentGames => _textPlayCurrentGames ??
                                                            (_textPlayCurrentGames = new ComponentResourceKey(typeof(XmalLanguageKeys), "TextPlayCurrentGames"));

        public static ComponentResourceKey TextRestoreVersion => _textRestoreVersion ??
                                                            (_textRestoreVersion = new ComponentResourceKey(typeof(XmalLanguageKeys), "TextRestoreVersion"));

        public static ComponentResourceKey TextRestoreInfo => _textRestoreInfo ??
                                                            (_textRestoreInfo = new ComponentResourceKey(typeof(XmalLanguageKeys), "TextRestoreInfo"));

        public static ComponentResourceKey TextDoNotShowAgain => _textDoNotShowAgain ??
                                                           (_textDoNotShowAgain = new ComponentResourceKey(typeof(XmalLanguageKeys), "TextDoNotShowAgain"));
        #endregion
    }
}
