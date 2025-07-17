using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Infocad.DynamicSpace.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Infocad.DynamicSpace.DynamicEntries;

public class EfCoreDynamicEntryRepository : EfCoreRepository<DynamicSpaceDbContext, DynamicEntry, Guid>,
    IDynamicEntryRepository
{
    public EfCoreDynamicEntryRepository(IDbContextProvider<DynamicSpaceDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }


    public async Task<List<DynamicEntry>> GetListByEntityAsync(Guid entityId, int skipCount, int maxResultCount, string sorting, string filter = null)
    {
        var dbSet = await GetDbSetAsync();
        var query = dbSet.Where(e => e.DynamicEntityId.Equals(entityId))
            .WhereIf(!filter.IsNullOrWhiteSpace(), entry => entry.ExtraProperties.ContainsValue(filter));

        if (!string.IsNullOrWhiteSpace(sorting))
            query = query.OrderBy(sorting);

        return await query.Skip(skipCount).Take(maxResultCount).ToListAsync();
    }
}