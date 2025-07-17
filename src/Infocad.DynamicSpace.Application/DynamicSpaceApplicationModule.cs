using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.Account;
using Volo.Abp.Identity;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;
using Volo.Abp.TenantManagement;
using Volo.Abp.BlobStoring;

namespace Infocad.DynamicSpace;

[DependsOn(
    typeof(DynamicSpaceDomainModule),
    typeof(DynamicSpaceApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpAccountApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule),
    (typeof(AbpBlobStoringModule))

    )]
[DependsOn(typeof(AbpBlobStoringModule))]
    public class DynamicSpaceApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<DynamicSpaceApplicationModule>();
        });
    }
}
