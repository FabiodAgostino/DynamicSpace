using Infocad.DynamicSpace.EntityFrameworkCore;
using Infocad.DynamicSpace.HybridCompanies;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Infocad.DynamicSpace.HybridCompanies
{
    public class EFCoreHybridCompanyRepository : EfCoreRepository<DynamicSpaceDbContext, HybridCompany, Guid>, IHybridCompanyRepository
    {
        public EFCoreHybridCompanyRepository(IDbContextProvider<DynamicSpaceDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<HybridCompany>> GetListByEntityAsync(Guid? entityId, int skipCount, int maxResultCount, string sorting, string filter = null)
        {
            var dbSet = await GetDbSetAsync();
            var query = dbSet.Where(e => !entityId.HasValue || e.DynamicEntityId.Equals(entityId))
                .WhereIf(!filter.IsNullOrWhiteSpace(), entry => entry.ExtraProperties.ContainsValue(filter))
                .Skip(skipCount)
                .Take(maxResultCount);

            return await query.ToListAsync();
        }
    }
}
