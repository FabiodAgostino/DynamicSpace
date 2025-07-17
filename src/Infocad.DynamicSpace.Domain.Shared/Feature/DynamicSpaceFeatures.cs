using System;

namespace Infocad.DynamicSpace.Feature
{
    /// <summary>
    /// Costanti per le feature definite nel sistema DynamicSpace
    /// </summary>
    public static class DynamicSpaceFeatures
    {
        /// <summary>
        /// Nome del gruppo principale delle feature (coerente con i permessi)
        /// </summary>
        public const string GroupName = "DynamicSpace";

        /// <summary>
        /// Feature per abilitare la generazione di report PDF
        /// </summary>
        public const string PdfReporting = GroupName + ".PdfReporting";

        /// <summary>
        /// Feature per definire il numero massimo di prodotti
        /// </summary>
        public const string MaxProductCount = GroupName + ".MaxProductCount";

        /// <summary>
        /// Feature per selezionare il tipo di export supportato
        /// </summary>
        public const string ExportType = GroupName + ".ExportType";

        /// <summary>
        /// Feature per abilitare l'accesso alle API esterne
        /// </summary>
        public const string ExternalApiAccess = GroupName + ".ExternalApiAccess";

        /// <summary>
        /// Feature per abilitare le funzionalità avanzate
        /// </summary>
        public const string AdvancedFeatures = GroupName + ".AdvancedFeatures";

        /// <summary>
        /// Feature per limitare il numero di utenti
        /// </summary>
        public const string MaxUserCount = GroupName + ".MaxUserCount";

        public const string FileManagement = GroupName + ".FileManagement";
        public const string AdvancedFileStorage = GroupName + ".AdvancedFileStorage";
    }
}