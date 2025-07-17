using Infocad.DynamicSpace.DynamicEntries;
using Infocad.DynamicSpace.DynamicRules;
using Infocad.DynamicSpace.HybridBuildings;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace Infocad.DynamicSpace.MongoDB.HybridBuildings
{
    public class MongoDbHybridBuildingRepository : MongoDbRepository<DynamicSpaceMongoDbContext, HybridBuilding, Guid>,
    IHybridBuildingRepository
    {
        public MongoDbHybridBuildingRepository(IMongoDbContextProvider<DynamicSpaceMongoDbContext> dbContextProvider) : base(
            dbContextProvider)
        {
        }

        public async Task<List<HybridBuilding>> GetListByEntityAsync(Guid? entityId, int skipCount, int maxResultCount, string sorting, string filter = null)
        {
            var dbContext = await GetDbContextAsync();

            var query = dbContext.HybridBuildings.AsQueryable()
                .Where(e => e.DynamicEntityId.Equals(entityId))
                .WhereIf(!filter.IsNullOrWhiteSpace(), entry => entry.ExtraProperties.ContainsValue(filter))
                .OrderBy(e => e.CreationTime)
                .Skip(skipCount)
                .Take(maxResultCount);

            return query.ToList();
        }
    }
}
