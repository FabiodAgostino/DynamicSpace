using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Infocad.DynamicSpace.DynamicHierarchies;

public interface IDynamicHierarchyRepository : IRepository<DynamicHierarchy, Guid>
{
    Task<DynamicHierarchy> FindByNameAsync(string name);

    Task<List<DynamicHierarchy>> GetListAsync(
        int skipCount,
        int maxResultCount,
        string sorting,
        string filter = null);

    Task<DynamicHierarchy> GetByIdIncludeEntitiesAsync(Guid Id);
}