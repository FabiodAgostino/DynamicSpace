using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.FeatureManagement;
using Volo.Abp.MultiTenancy;

namespace Infocad.DynamicSpace.Feature
{
    public class FeatureManagementAppService : DynamicSpaceAppService, IFeatureManagementAppService
    {
        private readonly IFeatureManager _featureManager;
        private readonly ICurrentTenant _tenant;

        private const string TenantProviderName = "T"; // Tenant provider
        private const string GlobalProviderName = "G"; // Global provider

        public FeatureManagementAppService(IFeatureManager featureManager, ICurrentTenant tenant)
        {
            _featureManager = featureManager;
            _tenant = tenant;
        }

        public async Task<TenantFeatureDto> GetAllTenantFeaturesAsync(Guid tenantId)
        {
            var tenantIdString = tenantId.ToString();

            var maxUsers = await _featureManager.GetOrNullAsync(DynamicSpaceFeatures.MaxUserCount, TenantProviderName, tenantIdString);
            var maxProducts = await _featureManager.GetOrNullAsync(DynamicSpaceFeatures.MaxProductCount, TenantProviderName, tenantIdString);
            var pdfReporting = await _featureManager.GetOrNullAsync(DynamicSpaceFeatures.PdfReporting, TenantProviderName, tenantIdString);
            var externalApiAccess = await _featureManager.GetOrNullAsync(DynamicSpaceFeatures.ExternalApiAccess, TenantProviderName, tenantIdString);
            var advancedFeatures = await _featureManager.GetOrNullAsync(DynamicSpaceFeatures.AdvancedFeatures, TenantProviderName, tenantIdString);

            bool? ParseNullableBool(string input) => bool.TryParse(input, out var result) ? result : null;

            var tenantFeature = new TenantFeatureDto
            {
                TenantId = tenantId,
                MaxUsers = maxUsers,
                MaxProducts = maxProducts,
                PdfReporting = ParseNullableBool(pdfReporting),
                ExternalApiAccess = ParseNullableBool(externalApiAccess),
                AdvancedFeatures = ParseNullableBool(advancedFeatures),
            };
            return tenantFeature;
        }

        public async Task<List<TenantFeatureDto>> GetAllTenantFeaturesBatchAsync(List<Guid> tenantIds)
        {
            var result = new List<TenantFeatureDto>();

            foreach (var tenantId in tenantIds)
            {
                try
                {
                    var tenantFeatures = await GetAllTenantFeaturesAsync(tenantId);
                    result.Add(tenantFeatures);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Errore nel caricamento delle feature per tenant {tenantId}: {ex.Message}");
                    result.Add(new TenantFeatureDto { TenantId = tenantId });
                }
            }

            return result;
        }


        public async Task SetTenantFeatureAsync(string featureName, string value, Guid? tenantId)
        {
            if (tenantId.HasValue)
            {
                await _featureManager.SetAsync(
                    featureName,
                    value,
                    TenantProviderName,
                    tenantId.Value.ToString());
            }
            else
            {
                throw new Exception("Nessun tenant attivo. Devi essere in un contesto tenant.");
            }
        }

        public async Task<string> GetTenantFeatureAsync(string featureName, Guid? tenantId)
        {
            if (tenantId.HasValue)
            {
                return await _featureManager.GetOrNullAsync(
                    featureName,
                    TenantProviderName,
                    tenantId.Value.ToString());
            }
            return null;
        }

        public async Task ConfigureBasicTenantPackageAsync(Guid? tenantId)
        {
            if (!tenantId.HasValue) return;

            var tenantIdString = tenantId.Value.ToString();

            // Imposta tutte le feature in una sola transazione
            var featureValues = new Dictionary<string, string>
            {
                { DynamicSpaceFeatures.PdfReporting, "false" },
                { DynamicSpaceFeatures.ExternalApiAccess, "false" },
                { DynamicSpaceFeatures.AdvancedFeatures, "false" },
                { DynamicSpaceFeatures.MaxProductCount, "50" },
                { DynamicSpaceFeatures.MaxUserCount, "25" },
                { DynamicSpaceFeatures.FileManagement, "true" },     
                { DynamicSpaceFeatures.AdvancedFileStorage, "false" }, 
            };

            await SetMultipleFeaturesAsync(tenantIdString, featureValues);
        }

        public async Task ConfigurePremiumTenantPackageAsync(Guid? tenantId)
        {
            if (!tenantId.HasValue) return;

            var tenantIdString = tenantId.Value.ToString();

            var featureValues = new Dictionary<string, string>
            {
                { DynamicSpaceFeatures.PdfReporting, "true" },
                { DynamicSpaceFeatures.ExternalApiAccess, "true" },
                { DynamicSpaceFeatures.AdvancedFeatures, "true" },
                { DynamicSpaceFeatures.MaxProductCount, "500" },
                { DynamicSpaceFeatures.MaxUserCount, "200" },
                { DynamicSpaceFeatures.FileManagement, "true" },       
                { DynamicSpaceFeatures.AdvancedFileStorage, "true" },  
            };

            await SetMultipleFeaturesAsync(tenantIdString, featureValues);
        }

        private async Task SetMultipleFeaturesAsync(string tenantId, Dictionary<string, string> featureValues)
        {
            foreach (var kvp in featureValues)
            {
                await _featureManager.SetAsync(kvp.Key, kvp.Value, TenantProviderName, tenantId);
            }
        }

        public async Task RemoveTenantFeaturesAsync(Guid? tenantId)
        {
            if (tenantId.HasValue)
            {
                await _featureManager.DeleteAsync(TenantProviderName, tenantId.Value.ToString());
            }
        }
    }

}