using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Infocad.DynamicSpace.HybridBuildings
{
    public interface IHybridBuildingRepository : IRepository<HybridBuilding, Guid>
    {
        Task<List<HybridBuilding>> GetListByEntityAsync(Guid? entityId, int skipCount, int maxResultCount, string sorting, string filter = null);
    }
}
