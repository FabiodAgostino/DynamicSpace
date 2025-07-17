using Infocad.DynamicSpace.EntityFrameworkCore;
using Infocad.DynamicSpace.MongoDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Autofac;
using Volo.Abp.Data;
using Volo.Abp.Modularity;

namespace Infocad.DynamicSpace.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
#if EFCORE
    typeof(DynamicSpaceEntityFrameworkCoreModule),
#else
    typeof(DynamicSpaceMongoDbModule),
#endif
    typeof(DynamicSpaceApplicationContractsModule)
)]
public class DynamicSpaceDbMigratorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        
        Configure<AbpDbConnectionOptions>(options =>
        {
#if EFCORE
            options.ConnectionStrings.Default = configuration.GetConnectionString("Default");
#else
            options.ConnectionStrings.Default = configuration.GetConnectionString("MongoDefault");
#endif
        });
    }
}