using System.Windows;

namespace RawLauncherWPF.Localization
{
    internal static class XmalLanguageKeys
    {
        private static ComponentResourceKey _buttonPlay;
        private static ComponentResourceKey _buttonOrganize;
        private static ComponentResourceKey _buttonPatch;
        private static ComponentResourceKey _buttonCheck;
        private static ComponentResourceKey _buttonLanguage;
        private static ComponentResourceKey _buttonRestore;
        private static ComponentResourceKey _buttonUpdate;
        private static ComponentResourceKey _buttonChangelog;
        private static ComponentResourceKey _buttonCancel;

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
    }
}
