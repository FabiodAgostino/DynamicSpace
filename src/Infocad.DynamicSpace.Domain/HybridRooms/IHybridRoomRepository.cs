using Infocad.DynamicSpace.HybridCompanies;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Infocad.DynamicSpace.HybridRooms
{
    public interface IHybridRoomRepository : IRepository<HybridRoom, Guid>
    {
        Task<List<HybridRoom>> GetListByEntityAsync(Guid? entityId, int skipCount, int maxResultCount, string sorting, string filter = null);
    }
}
