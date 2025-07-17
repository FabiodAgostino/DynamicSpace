using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infocad.DynamicSpace.Feature;
using Shouldly;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement;
using Volo.Abp.MultiTenancy;
using Xunit;

namespace Infocad.DynamicSpace.Feature
{
    public abstract class FeatureManagementService_Tests<TStartupModule> : DynamicSpaceApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IFeatureManagementAppService _featureManagementService;
        private readonly ITenantRepository _tenantRepository;

        protected FeatureManagementService_Tests()
        {
            _featureManagementService = GetRequiredService<IFeatureManagementAppService>();
            _tenantRepository = GetRequiredService<ITenantRepository>();
        }

        [Fact]
        public async Task Should_Set_And_Get_Tenant_Feature()
        {
            // Arrange
            var tenant = await CreateTestTenantAsync("Test Tenant");
            var featureName = DynamicSpaceFeatures.MaxUserCount;
            var featureValue = "100";

            // Act
            await _featureManagementService.SetTenantFeatureAsync(featureName, featureValue, tenant.Id);
            var result = await _featureManagementService.GetTenantFeatureAsync(featureName, tenant.Id);

            // Assert
            result.ShouldBe(featureValue);
        }

        [Fact]
        public async Task Should_Configure_Basic_Package()
        {
            // Arrange
            var tenant = await CreateTestTenantAsync("Basic Package Tenant");

            // Act
            await _featureManagementService.ConfigureBasicTenantPackageAsync(tenant.Id);

            // Assert
            var features = await _featureManagementService.GetAllTenantFeaturesAsync(tenant.Id);
            features.MaxUsers.ShouldBe("25");
            features.MaxProducts.ShouldBe("50");
            features.PdfReporting.ShouldBe(false);
            features.ExternalApiAccess.ShouldBe(false);
            features.AdvancedFeatures.ShouldBe(false);
            features.ExportType.ShouldBe("Excel");
            features.GetCurrentPackage().ShouldBe("Basic");
        }

        [Fact]
        public async Task Should_Configure_Premium_Package()
        {
            // Arrange
            var tenant = await CreateTestTenantAsync("Premium Package Tenant");

            // Act
            await _featureManagementService.ConfigurePremiumTenantPackageAsync(tenant.Id);

            // Assert
            var features = await _featureManagementService.GetAllTenantFeaturesAsync(tenant.Id);
            features.MaxUsers.ShouldBe("200");
            features.MaxProducts.ShouldBe("500");
            features.PdfReporting.ShouldBe(true);
            features.ExternalApiAccess.ShouldBe(true);
            features.AdvancedFeatures.ShouldBe(true);
            features.ExportType.ShouldBe("Json");
            features.GetCurrentPackage().ShouldBe("Premium");
        }


        [Fact]
        public async Task Should_Get_All_Tenant_Features()
        {
            // Arrange
            var tenant = await CreateTestTenantAsync("All Features Tenant");
            await _featureManagementService.SetTenantFeatureAsync(DynamicSpaceFeatures.MaxUserCount, "150", tenant.Id);
            await _featureManagementService.SetTenantFeatureAsync(DynamicSpaceFeatures.PdfReporting, "true", tenant.Id);

            // Act
            var result = await _featureManagementService.GetAllTenantFeaturesAsync(tenant.Id);

            // Assert
            result.ShouldNotBeNull();
            result.TenantId.ShouldBe(tenant.Id);
            result.MaxUsers.ShouldBe("150");
            result.PdfReporting.ShouldBe(true);
        }

        [Fact]
        public async Task Should_Get_All_Tenant_Features_Batch()
        {
            // Arrange
            var tenant1 = await CreateTestTenantAsync("Batch Tenant 1");
            var tenant2 = await CreateTestTenantAsync("Batch Tenant 2");

            await _featureManagementService.ConfigureBasicTenantPackageAsync(tenant1.Id);
            await _featureManagementService.ConfigurePremiumTenantPackageAsync(tenant2.Id);

            var tenantIds = new List<Guid> { tenant1.Id, tenant2.Id };

            // Act
            var result = await _featureManagementService.GetAllTenantFeaturesBatchAsync(tenantIds);

            // Assert
            result.Count.ShouldBe(2);

            var tenant1Features = result.FirstOrDefault(x => x.TenantId == tenant1.Id);
            tenant1Features.ShouldNotBeNull();
            tenant1Features.GetCurrentPackage().ShouldBe("Basic");

            var tenant2Features = result.FirstOrDefault(x => x.TenantId == tenant2.Id);
            tenant2Features.ShouldNotBeNull();
            tenant2Features.GetCurrentPackage().ShouldBe("Premium");
        }

        [Fact]
        public async Task Should_Handle_Custom_Package_Configuration()
        {
            // Arrange
            var tenant = await CreateTestTenantAsync("Custom Package Tenant");

            await _featureManagementService.SetTenantFeatureAsync(DynamicSpaceFeatures.MaxUserCount, "75", tenant.Id);
            await _featureManagementService.SetTenantFeatureAsync(DynamicSpaceFeatures.MaxProductCount, "300", tenant.Id);
            await _featureManagementService.SetTenantFeatureAsync(DynamicSpaceFeatures.PdfReporting, "true", tenant.Id);
            await _featureManagementService.SetTenantFeatureAsync(DynamicSpaceFeatures.ExternalApiAccess, "false", tenant.Id);

            // Assert
            var features = await _featureManagementService.GetAllTenantFeaturesAsync(tenant.Id);
            features.GetCurrentPackage().ShouldBe("Custom");
        }

        [Fact]
        public async Task Should_Override_Basic_With_Premium_Package()
        {
            // Arrange
            var tenant = await CreateTestTenantAsync("Override Package Tenant");

            await _featureManagementService.ConfigureBasicTenantPackageAsync(tenant.Id);
            var basicFeatures = await _featureManagementService.GetAllTenantFeaturesAsync(tenant.Id);
            basicFeatures.GetCurrentPackage().ShouldBe("Basic");

            await _featureManagementService.ConfigurePremiumTenantPackageAsync(tenant.Id);

            // Assert
            var premiumFeatures = await _featureManagementService.GetAllTenantFeaturesAsync(tenant.Id);
            premiumFeatures.GetCurrentPackage().ShouldBe("Premium");
            premiumFeatures.MaxUsers.ShouldBe("200");
            premiumFeatures.PdfReporting.ShouldBe(true);
        }

        [Fact]
        public async Task Should_Parse_Boolean_Features_Correctly()
        {
            // Arrange
            var tenant = await CreateTestTenantAsync("Boolean Features Tenant");

            // Act
            await _featureManagementService.SetTenantFeatureAsync(DynamicSpaceFeatures.PdfReporting, "true", tenant.Id);
            await _featureManagementService.SetTenantFeatureAsync(DynamicSpaceFeatures.ExternalApiAccess, "false", tenant.Id);

            // Assert
            var features = await _featureManagementService.GetAllTenantFeaturesAsync(tenant.Id);
            features.PdfReporting.ShouldBe(true);
            features.ExternalApiAccess.ShouldBe(false);
        }

        [Fact]
        public async Task Should_Handle_Empty_Tenant_List_In_Batch()
        {
            // Arrange
            var emptyTenantIds = new List<Guid>();

            // Act
            var result = await _featureManagementService.GetAllTenantFeaturesBatchAsync(emptyTenantIds);

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(0);
        }

        [Fact]
        public async Task Should_Return_Default_Values_From_Provider()
        {
            // Arrange
            var tenant = await CreateTestTenantAsync("Default Values Tenant");

            // Act
            var features = await _featureManagementService.GetAllTenantFeaturesAsync(tenant.Id);

            // Assert
            features.ShouldNotBeNull();
            features.TenantId.ShouldBe(tenant.Id);

            features.MaxUsers.ShouldBe("50"); 
            features.MaxProducts.ShouldBe("100"); 
            features.PdfReporting.ShouldBe(false); 
            features.ExternalApiAccess.ShouldBe(false); 
            features.ExportType.ShouldBe("Excel");

            features.GetCurrentPackage().ShouldBe("Custom");
        }

        private async Task<Tenant> CreateTestTenantAsync(string name)
        {
            var tenantManager = GetRequiredService<ITenantManager>();
            var tenant = await tenantManager.CreateAsync(name);
            await _tenantRepository.InsertAsync(tenant, autoSave: true);
            return tenant;
        }
    }
}