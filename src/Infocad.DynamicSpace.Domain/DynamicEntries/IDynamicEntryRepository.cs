using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Infocad.DynamicSpace.DynamicEntries;

public interface IDynamicEntryRepository : IRepository<DynamicEntry,Guid>
{
    Task<List<DynamicEntry>> GetListByEntityAsync(Guid entityId, int skipCount, int maxResultCount,
        string sorting, string filter = null );
}