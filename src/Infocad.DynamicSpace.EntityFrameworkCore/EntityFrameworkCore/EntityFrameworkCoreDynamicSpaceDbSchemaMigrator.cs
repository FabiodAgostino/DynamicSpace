using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Infocad.DynamicSpace.Data;
using Volo.Abp.DependencyInjection;

namespace Infocad.DynamicSpace.EntityFrameworkCore;

public class EntityFrameworkCoreDynamicSpaceDbSchemaMigrator
    : IDynamicSpaceDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreDynamicSpaceDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the DynamicSpaceDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<DynamicSpaceDbContext>()
            .Database
            .MigrateAsync();
    }
}
