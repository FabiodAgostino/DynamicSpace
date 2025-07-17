using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Infocad.DynamicSpace.Feature
{
    public interface IFeatureManagementAppService : IApplicationService
    {
        Task SetTenantFeatureAsync(string featureName, string value, Guid? tenantId);
        Task<string> GetTenantFeatureAsync(string featureName, Guid? tenantId);
        Task ConfigureBasicTenantPackageAsync(Guid? tenantId);
        Task ConfigurePremiumTenantPackageAsync(Guid? tenantId);
        Task RemoveTenantFeaturesAsync(Guid? tenantId);
        Task<TenantFeatureDto> GetAllTenantFeaturesAsync(Guid tenantId);
        Task<List<TenantFeatureDto>> GetAllTenantFeaturesBatchAsync(List<Guid> tenantIds);
    }
}