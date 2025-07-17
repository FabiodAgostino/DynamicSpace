using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Infocad.DynamicSpace.Data;

/* This is used if database provider does't define
 * IDynamicSpaceDbSchemaMigrator implementation.
 */
public class NullDynamicSpaceDbSchemaMigrator : IDynamicSpaceDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
