using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infocad.DynamicSpace.DynamicAttirbutes;
using Infocad.DynamicSpace.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Infocad.DynamicSpace.DynamicAttributes;

public class EFCoreDynamicAttributeRepository : EfCoreRepository<DynamicSpaceDbContext,DynamicAttribute,Guid> , IDynamicAttributeRepository
{
    public EFCoreDynamicAttributeRepository(IDbContextProvider<DynamicSpaceDbContext> dbContextProvider) : base(
        dbContextProvider)
    {
        
    }

    public async Task<List<DynamicAttribute>> GetListByGuidsAsync(List<Guid> attributeIds)
    {
        var dbContext = await GetDbContextAsync();
        var query = dbContext.Set<DynamicAttribute>().AsQueryable();
        return await query.Where(attribute => attributeIds.Contains(attribute.Id))
                                .ToListAsync();
    }
}