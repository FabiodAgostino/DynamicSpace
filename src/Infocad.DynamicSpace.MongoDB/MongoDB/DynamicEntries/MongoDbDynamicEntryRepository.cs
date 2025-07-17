using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infocad.DynamicSpace.DynamicEntries;
using MongoDB.Driver;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace Infocad.DynamicSpace.MongoDB.DynamicEntries;

public class MongoDbDynamicEntryRepository : MongoDbRepository<DynamicSpaceMongoDbContext, DynamicEntry, Guid>, IDynamicEntryRepository
{
    public MongoDbDynamicEntryRepository(IMongoDbContextProvider<DynamicSpaceMongoDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<List<DynamicEntry>> GetListByEntityAsync(Guid entityId, int skipCount, int maxResultCount, string sorting, string filter = null)
    {
        var dbContext = await GetDbContextAsync();

        var query = dbContext.DynamicEntries.AsQueryable()
            .Where(e => e.DynamicEntityId.Equals(entityId))
            .WhereIf(!filter.IsNullOrWhiteSpace(), entry => entry.ExtraProperties.ContainsValue(filter))
            .OrderBy(e => e.CreationTime)
            .Skip(skipCount)
            .Take(maxResultCount);

        return query.ToList();
    }
}