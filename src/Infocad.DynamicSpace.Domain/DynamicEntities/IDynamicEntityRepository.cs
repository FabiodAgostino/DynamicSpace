using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Infocad.DynamicSpace.DynamicEntities
{
    public interface IDynamicEntityRepository: IRepository<DynamicEntity,Guid>
    {
        Task<DynamicEntity> FindByNameAsync(string name);
        
        Task<List<DynamicEntity>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string sorting,
            string filter = null
        );

        Task<List<DynamicEntity>> GetListByDynamicTypeAsync(
            Guid typeId);
        Task<List<DynamicEntity>> GetHybridEntitiesUsed();
        Task<DynamicEntity> GetByIdIncludeAttributeAsync(Guid Id);
        Task<DynamicEntity?> GetFullEntityByHybridEntity(string hybridTypeName);

    }
}
