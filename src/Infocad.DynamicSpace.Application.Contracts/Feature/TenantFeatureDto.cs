using System;

namespace Infocad.DynamicSpace.Feature
{
    /// <summary>
    /// DTO per contenere tutte le feature di un tenant
    /// </summary>
    public class TenantFeatureDto
    {
        public Guid TenantId { get; set; }
        public string MaxUsers { get; set; }
        public string MaxProducts { get; set; }
        public bool? PdfReporting { get; set; }
        public bool? ExternalApiAccess { get; set; }
        public bool? AdvancedFeatures { get; set; }
        public string ExportType { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? NormalizedName { get; set; }
        public string? CurrentPackage { get; set; }
        public bool IsLoading { get; set; }
        public string LoadingAction { get; set; } = string.Empty;

        /// <summary>
        /// Determina il pacchetto attuale basandosi sui valori delle feature
        /// </summary>
        public string GetCurrentPackage()
        {
            if (string.IsNullOrEmpty(MaxUsers)) return null;

            // Logic per determinare il pacchetto basandosi sui valori delle feature
            if (MaxUsers == "25" && MaxProducts == "50" &&
                PdfReporting == false && ExternalApiAccess == false)
            {
                return "Basic";
            }
            else if (MaxUsers == "200" && MaxProducts == "500" &&
                     PdfReporting == true && ExternalApiAccess == true)
            {
                return "Premium";
            }

            return "Custom";
        }
    }
}
