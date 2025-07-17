using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infocad.DynamicSpace.DynamicEntityAttributes;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Infocad.DynamicSpace.DynamicEntities
{
    public interface IDynamicEntityService : IApplicationService
    {
        Task<List<DynamicEntityDto>> GetListByDynamicTypeAsync(Guid id);
        Task<PagedResultDto<DynamicEntityDto>> GetListAsync(GetDynamicEntityListDto input);
        Task<DynamicEntityDto> GetByIdIncludeAttributeAsync(Guid id);
        Task<DynamicEntityDto> CreateAsync(CreateDynamicEntityDto input);
        public Task<DynamicEntityDto> UpdateAttribute(UpdateDynamicEntityAttributeDto input);
        public Task<DynamicEntityDto> CreateAttributeAsync(CreateDynamicEntityAttributeDto input);
        public Task<DynamicEntityDto> DeleteAttributeAsync(Guid idEntity, Guid idAttribute);
        public Task UpdateAsync(UpdateDynamicEntityDto input);
        Task DeleteAsync(Guid id);
        Task<DynamicEntityDto?> GetFullEntityByHybridEntity(string hybridTypeName);
        Task<List<DynamicEntityDto>> GetHybridEntitiesUsed();
        Task<List<DynamicEntityDto>> GetHybridEntities();
        Task<List<DynamicEntityDto>> GetNoHybridEntities();
        Task<List<DynamicEntityDto>> GetAllDynamicEntity();
    }

}
