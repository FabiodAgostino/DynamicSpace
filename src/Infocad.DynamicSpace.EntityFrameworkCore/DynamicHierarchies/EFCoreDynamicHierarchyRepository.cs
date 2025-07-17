using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Dynamic.Core;
using Infocad.DynamicSpace.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Infocad.DynamicSpace.DynamicHierarchies;

public class EFCoreDynamicHierarchyRepository : EfCoreRepository<DynamicSpaceDbContext,DynamicHierarchy,Guid> , IDynamicHierarchyRepository
{
    public EFCoreDynamicHierarchyRepository(IDbContextProvider<DynamicSpaceDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
    
    public async Task<DynamicHierarchy> GetByIdIncludeEntitiesAsync(Guid Id)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Include("Entities")
            .FirstOrDefaultAsync(entity => Id != Guid.Empty && entity.Id == Id);
    }

    
    public async Task<List<DynamicHierarchy>> GetListAsync(
        int skipCount,
        int maxResultCount,
        string sorting,
        string filter = null)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .WhereIf(
                !filter.IsNullOrWhiteSpace(),
                dynamic => dynamic.Name.Contains(filter)
            )
            .Include("Entities")
            .OrderBy(sorting)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync();
    }

    public async Task<DynamicHierarchy> FindByNameAsync(string name)
    {
        return await (await GetDbSetAsync()).FirstOrDefaultAsync(h => h.Name == name);
    }
}