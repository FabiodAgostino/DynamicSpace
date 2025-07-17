using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Infocad.DynamicSpace.DynamicHierarchies;

public interface IDynamicHierarchyService : IApplicationService
{
    Task<DynamicHierarchyDto> GetAsync(Guid id);
    Task<DynamicHierarchyDto> CreateAsync(CreateDynamicHierarchyDto input);
    Task<DynamicHierarchyDto> UpdateAsync(Guid id, CreateDynamicHierarchyDto input);
    Task DeleteAsync(Guid id);
    Task<DynamicHierarchyDto> CreateDynamicHiererchyEntityAsync(Guid id, CreateDynamicHiererchyEntityDto input);
    Task<DynamicHierarchyDto> UpdateDynamicHiererchyEntityAsync(Guid id, UpdateDynamicHierarchyEntityDto input);
    Task DeleteDynamicHierarchyEntityAsync(Guid id, Guid entityId);
    Task<PagedResultDto<DynamicHierarchyDto>> GetListAsync(GetDyanmicHierarchyListDto input);
    Task<List<DynamicHierarchyEntityDto>> GetDynamicHierarchyEntitiesAsync(Guid hierarchyId);
    Task DeleteAllHierarchyEntityAsync(Guid id);
}