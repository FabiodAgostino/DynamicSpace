using Infocad.DynamicSpace.HybridCompanies;
using Infocad.DynamicSpace.HybridRooms;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace Infocad.DynamicSpace.MongoDB.HybridRooms
{
    public class MongoDbHybridRoomRepository : MongoDbRepository<DynamicSpaceMongoDbContext, HybridRoom, Guid>,
    IHybridRoomRepository
    {
        public MongoDbHybridRoomRepository(IMongoDbContextProvider<DynamicSpaceMongoDbContext> dbContextProvider) : base(
            dbContextProvider)
        {
        }

        public async Task<List<HybridRoom>> GetListByEntityAsync(Guid? entityId, int skipCount, int maxResultCount, string sorting, string filter = null)
        {
            var dbContext = await GetDbContextAsync();

            var query = dbContext.HybridRooms.AsQueryable()
                .Where(e => e.DynamicEntityId.Equals(entityId))
                .WhereIf(!filter.IsNullOrWhiteSpace(), entry => entry.ExtraProperties.ContainsValue(filter))
                .OrderBy(e => e.CreationTime)
                .Skip(skipCount)
                .Take(maxResultCount);

            return query.ToList();
        }
    }
}
