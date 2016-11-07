using System;
using System.Collections.Generic;
using System.Windows;

namespace RawLauncherWPF.Localization
{
    sealed class Spanish : Language
    {
        public Spanish()
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
                Source = new Uri("/RawLauncherWPF;component/Localization/Spanish.xaml", UriKind.Relative)
            });
        }

        private void AddCheckStrigns()
        {
            StringTable.Add("CheckFolderNotValid", "Directorio no válido");
            StringTable.Add("CheckAIFolderNotValid", "El directorio 'data' de su juego no coincide con su versión actualmente instalada de Republic at War.\r\n\r\n"
                                                     + "¿Desea ver el informe de error detallado ahora?");
            StringTable.Add("CheckModFolderNotValid", "El directorio de Republic at War no coincide con su versión actualmente instalada de Republic at War.\r\n\r\n"
                                                     + "¿Desea ver el informe de error detallado ahora?");
            StringTable.Add("CheckFolderNotValidCount", "El directorio siguiente contiene demasiados o demasiado archivos: {0}");
            StringTable.Add("CheckFolderNotValidExists", "El directorio siguiente no existe: {0}");
            StringTable.Add("CheckFolderNotValidHash", "El directorio siguiente contiene archivos no válidos: {0}");
            StringTable.Add("CheckGamesNotPatchedMessage", "Fuerzas de la Corrupción no está actualizado. Por favor presione 'parchear' para actualizar su juego.");
            StringTable.Add("CheckMessageAborted", "abortado");
            StringTable.Add("CheckMessageAiCorrect", "ia configurada");
            StringTable.Add("CheckMessageAiWrong", "ia desconfigurada");
            StringTable.Add("CheckMessageCouldNotCheck", "imposible verificar");
            StringTable.Add("CheckMessageGameFound", "foc encontrado");
            StringTable.Add("CheckMessageGameNotFound", "foc no encontrado");
            StringTable.Add("CheckMessageGamesNotPatched", "juego no parcheado");
            StringTable.Add("CheckMessageGamesPatched", "juego parcheado");
            StringTable.Add("CheckMessageModCorrect", "mod configurado");
            StringTable.Add("CheckMessageModFound", "raw encontrado");
            StringTable.Add("CheckMessageModNotFound", "raw no encontrado");
            StringTable.Add("CheckMessageModWrong", "raw desconfigurado");
            StringTable.Add("CheckOfflineXmlNotFound", "Revisión de versión fallida. Por favor haga clic en la pestaña 'Restaurar' y restaure los archivos del mod.");
            StringTable.Add("CheckPatchGamesFailed", "Parcheo del juego fallido.");
            StringTable.Add("CheckPatchGamesFailedBase", "Parcheo de Fuerzas de la Corrupción fallido. Imperio en Guerra parcheado exitosamente.");
            StringTable.Add("CheckPatchGamesFailedEaw", "Parcheo de Fuerzas de la Corrupción fallido. Imperio en Guerra parcheado exitosamente.");
            StringTable.Add("CheckPatchGamesSuccess", "Juego parcheado exitosamente.");
            StringTable.Add("CheckStatusChecking", "Verificando: {0}");
            StringTable.Add("CheckStatusCheckingGameExist", "Buscando FoC");
            StringTable.Add("CheckStatusCheckingGamePatches", "Buscando parches para el juego");
            StringTable.Add("CheckStatusCheckingModExist", "Buscando RaW");
            StringTable.Add("CheckStatusPrepareAiModCheck", "Preparando IA/verificando mod");
            StringTable.Add("CheckVersionNotFound", "No se puede recuperar la versión especificada de los servidores remotos. Por favor trate de nuevo mas tarde.");
            StringTable.Add("CheckXmlValidateError", "Los archivos XML necesarios no son válidos. Por favor, haga clic en la pestaña 'Restaurar' y restaure los archivos del mod.");
            StringTable.Add("CheckXmlWrongVersion", "Esta versión del mod no coincide con el archivo de referencia. Por favor, haga clic en la pestaña 'Restaurar' y restaure los archivos del mod.");
        }

        private void AddErrorStrings()
        {
            StringTable.Add("ErrorAlreadyRunning", " Ya se está ejecutando otra instancia del lanzador.");
            StringTable.Add("ErrorCreateMessageFailed", "No se puede crear mensaje de texto.");
            StringTable.Add("ErrorInitLauncher", "Hubo un problema al inicializar el lanzador: {0}");
            StringTable.Add("ErrorInitFailed", "El lanzador no puede localizar Fuerzas de la Corrupción. Por favor ejecute el lanzador desde el directorio raíz de Fuerzas de la Corrupción.");
            StringTable.Add("ErrorInitFailedMod", "El lanzador no puede localizar Republic at War. Por favor haga clic en la pestaña 'Actualizar' e instale el mod.");
        }

        private void AddExceptionStrings()
        {
            StringTable.Add("ExceptionGameExist", "No se pudo encontrar el juego.");
            StringTable.Add("ExceptionGameExistName", "{0} no pudo ser encontrado.");
            StringTable.Add("ExceptionGameModCompatible", "Mod no es compatible");
            StringTable.Add("ExceptionGameModExist", "No se pudo encontrar el mod.");
            StringTable.Add("ExceptionGameModWrongInstalled", "El mod no está instalado.");
            StringTable.Add("ExceptionHostServerGetData", "No se pudieron obtener datos de: {0}");
            StringTable.Add("ExceptionModExist", "No se pudo encontrar el mod.");
            StringTable.Add("ExceptionModExistName", "{0} no pudo ser encontrado.");
            StringTable.Add("ExceptionResourceExtractorNotFound", "El recurso no pudo ser encontrado: {0}");
            StringTable.Add("ExceptionRestoreVersionNotMatch", "Las versiones no concuerdan");
            StringTable.Add("ExceptionUpdateVersionNotMatch", "Las versiones no concuerdan");
            StringTable.Add("ExceptionXmlParserError", "No se ha podido deserializar el flujo xml. {0}");
            StringTable.Add("ExceptionSteamClientMissing", "No se ha podido encontrar la aplicación cliente de Steam");
        }

        private void AddLanguageStrings()
        {
            StringTable.Add("LanguageAdditionalSupport", "Hay una versión específica separada disponible para este idioma. Por favor verifique Moddb.com para obtener un Paquete de Idioma o revise el instalador de RaW para más opciones.");
            StringTable.Add("LanguageMessageChangedSuccess", "Lenguaje cambiado exitosamente.");
            StringTable.Add("LanguageMessageSpeechFileRenameFailed", "No se pudo actualizar Speech.meg.");
            StringTable.Add("LanguageMessageSpeechRenameFailed", "No se pudo actualizar el directorio Speech.");
            StringTable.Add("LanguageMessageTextRenameFailed", "No se pudo actualizar MasterTextFile.");
            StringTable.Add("LanguageNoneSelected", "Por favor seleccione un lenguaje.");
            StringTable.Add("LangugeOperationQuestion", "Tenga en cuenta que el cambio de idioma significa que cualquier [MISSING]s y audio faltante será reemplazado por la versión en Inglés por defecto. Para cualquier otro paquete de idioma, considere nuestra página en ModDB o revise las opciones de idiomas del instalador de Republic at War.");
        }

        private void AddLauncherStrings()
        {
            StringTable.Add("LauncherInfoNewVersion", "Nueva versión: {0} disponible.");
        }

        private void AddModStrings()
        {
            StringTable.Add("ModVersionNotFound", "No se pudo encontrar la versión actual. Por favor reinstale Republic at War e intente nuevamente.");
            StringTable.Add("UninstallModWarning", "¿Está seguro de que quiere eliminar Republic at War ? Esto no se puede deshacer.");
        }

        private void AddPlayStrings()
        {
            StringTable.Add("PlayCurrentSessionWait", "espere");
        }

        private void AddResourceExtractorStrings()
        {
            StringTable.Add("ResourceExtractorNewDirectoryCreated", "Nuevo directorio creado: {0}");
        }

        private void AddRestoreStrigns()
        {
            StringTable.Add("RestoreAborted", "Restauración abortada");
            StringTable.Add("RestoreDone", "Restauración lista");
            StringTable.Add("RestoreErrorExit", "Proceso abortado o fallido.");
            StringTable.Add("RestoreHostServerOffline", "No se puede descargar los archivos necesarios ya que los servidores no pudieron ser contactados. Por favor intente mas tarde.");
            StringTable.Add("RestoreInternetLost", "Ha perdido su conexión a Internet. Con el fin de evitar que aparezcan más mensajes de error, el progreso se cancelará ahora.");
            StringTable.Add("RestoreNoInternet", "Necesita una conexión a Internet para ejecutar una restauración.");
            StringTable.Add("RestoreNoVersion", "Por favor, seleccione una versión para restaurar.");
            StringTable.Add("RestoreOperationQuestion", "¿Seguro que desea restaurar Republic at War? Esto no se puede deshacer. Los archivos modificados serán borrados y restaurados a su estado predeterminado.");
            StringTable.Add("RestoreStatusCheckAdditionalFiles", "Comprobando archivos adicionales.");
            StringTable.Add("RestoreStatusCheckMissing", "Comprobando archivos faltantes/corruptos.");
            StringTable.Add("RestoreStatusDownloaded", "Descargado: {0}.");
            StringTable.Add("RestoreStatusFinishing", "Finalizando.");
            StringTable.Add("RestoreStatusPrepare", "Preparando restauración.");
            StringTable.Add("RestoreStatusPrepareTable", "Preparando tabla de descarga.");
            StringTable.Add("RestoreStautsDeletingMod", "Borrando archivos del mod.");
            StringTable.Add("RestoreStreamNull", "Error al descargar los archivos necesarios. Por favor intente mas tarde.");
            StringTable.Add("RestoreTableNull", "Error al intentar descargar los archivos del mod. La tabla requerida estaba vacía. Por favor, inténtelo de nuevo.");
            StringTable.Add("RestoreVersionNotMatch", "No se puede recuperar la versión especificada de los servidores remotos. Por favor intente mas tarde.");
            StringTable.Add("RestoreXmlNotValid", "Los archivos XML necesarios no son válidos. Por favor, haga clic en la pestaña 'Restaurar' y restaure los archivos del mod.");
        }

        private void AddUpdateStrings()
        {
            StringTable.Add("UpdateAborted", "Actualización abortada.");
            StringTable.Add("UpdateDone", "Actualización completa.");
            StringTable.Add("UpdateErrorExit", "Proceso abortado o fallido.");
            StringTable.Add("UpdateHostOffline", "No se puede descargar los archivos necesarios ya que los servidores no pudieron ser contactados. Por favor intente mas tarde.");
            StringTable.Add("UpdateInternetLost", "Ha perdido su conexión a Internet. Con el fin de evitar que aparezcan más mensajes de error, el progreso se cancelará ahora.");
            StringTable.Add("UpdateNoInternet", "Necesita una conexión a internet para actualizar Republic at War.");
            StringTable.Add("UpdateOperationQuestion", "¿Seguro que desea actualizar Republic at War? Esto no se puede deshacer. Los archivos modificados serán borrados y restaurados a su estado predeterminado.");
            StringTable.Add("UpdateStatusCheckAdditionalFiles", "Comprobando archivos adicionales");
            StringTable.Add("UpdateStatusCheckNew", "Comprobando archivos nuevos/corruptos");
            StringTable.Add("UpdateStatusDownloaded", "Descargado: {0}.");
            StringTable.Add("UpdateStatusFinishing", "Terminando.");
            StringTable.Add("UpdateStatusPrepare", "Preparando actualización.");
            StringTable.Add("UpdateStreamNull", "Error al descargar los archivos necesarios. Por favor intente mas tarde.");
            StringTable.Add("UpdateTableNull", "Error al intentar descargar los archivos del mod. La tabla requerida estaba vacía. Por favor, inténtelo de nuevo.");
            StringTable.Add("UpdateVersionNotFound", "No se puede recuperar la versión especificada de los servidores remotos. Por favor intente mas tarde.");
            StringTable.Add("UpdateXmlNotValid", "Los archivos XML necesarios no son válidos. Por favor, haga clic en la pestaña 'Restaurar' y restaure los archivos del mod.");
        }

        private void AddVersionStrings()
        {
            StringTable.Add("VersionUtilitiesAskForUpdate", "Nueva versión: {0} disponible. ¿Actualizar ahora?");
        }
    }
}
