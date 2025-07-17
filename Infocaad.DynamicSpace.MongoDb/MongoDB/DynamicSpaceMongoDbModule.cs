using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;

namespace Infocaad.DynamicSpace.MongoDB;

[DependsOn(
    typeof(AbpMongoDbModule)
)]
public class DynamicSpaceMongoDbModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMongoDbContext<DynamicSpaceMongoDbContext>(options =>
        {
            options.AddDefaultRepositories();
            
            /* Add custom repositories here. Example:
             * options.AddRepository<Question, MongoQuestionRepository>();
             */
        });
    }
}
