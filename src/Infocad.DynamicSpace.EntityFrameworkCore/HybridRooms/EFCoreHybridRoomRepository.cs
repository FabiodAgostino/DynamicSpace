using Infocad.DynamicSpace.EntityFrameworkCore;
using Infocad.DynamicSpace.HybridCompanies;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Infocad.DynamicSpace.HybridRooms
{
    public class EFCoreHybridRoomRepository : EfCoreRepository<DynamicSpaceDbContext, HybridRoom, Guid>, IHybridRoomRepository
    {
        public EFCoreHybridRoomRepository(IDbContextProvider<DynamicSpaceDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<HybridRoom>> GetListByEntityAsync(Guid? entityId, int skipCount, int maxResultCount, string sorting, string filter = null)
        {
            var dbSet = await GetDbSetAsync();

            var baseQuery = dbSet.Where(e => !entityId.HasValue || e.DynamicEntityId.Equals(entityId));

            var query = baseQuery.AsEnumerable()
                .Where(entry => filter.IsNullOrWhiteSpace() || entry.ExtraProperties.ContainsValue(filter))
                .Skip(skipCount)
                .Take(maxResultCount);

            return query.ToList();
        }
    }
}
