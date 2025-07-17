using Infocad.DynamicSpace.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace Infocad.DynamicSpace.DynamicHierarchies;

public class DynamicHierarchyService : DynamicSpaceAppService, IDynamicHierarchyService
{
    private readonly IDynamicHierarchyRepository _dynamicHierarchyRepository;
    private readonly DynamicHierarchyManager _dynamicHierarchyManager;

    public DynamicHierarchyService(IDynamicHierarchyRepository dynamicHierarchyRepository,
        DynamicHierarchyManager dynamicHierarchyManager) : base()
    {
        _dynamicHierarchyRepository = dynamicHierarchyRepository;
        _dynamicHierarchyManager = dynamicHierarchyManager;
    }


    public async Task<PagedResultDto<DynamicHierarchyDto>> GetListAsync(GetDyanmicHierarchyListDto input)
    {
        if (input.Sorting.IsNullOrWhiteSpace())
        {
            input.Sorting = nameof(DynamicHierarchyDto.Name);
        }

        var dynamicEntities = await _dynamicHierarchyRepository.GetListAsync(
            input.SkipCount,
            input.MaxResultCount,
            input.Sorting,
            input.Filter
        );

        var totalCount = input.Filter == null
            ? await _dynamicHierarchyRepository.CountAsync()
            : await _dynamicHierarchyRepository.CountAsync(
                dynamicEntity => dynamicEntity.Name.Contains(input.Filter));

        return new PagedResultDto<DynamicHierarchyDto>(
            totalCount,
            ObjectMapper.Map<List<DynamicHierarchy>, List<DynamicHierarchyDto>>(dynamicEntities)
        );
    }

    public async Task<DynamicHierarchyDto> GetAsync(Guid id)
    {
        var dynamicHierarchy = await _dynamicHierarchyRepository.GetAsync(id);
        return ObjectMapper.Map<DynamicHierarchy, DynamicHierarchyDto>(dynamicHierarchy);
    }

    public async Task<DynamicHierarchyDto> CreateAsync(CreateDynamicHierarchyDto input)
    {
        var dynamicHierarchy = await _dynamicHierarchyManager.CreateAsync(input.Name,
            input.Description,
            input.Default);
        _dynamicHierarchyRepository.InsertAsync(dynamicHierarchy);
        return ObjectMapper.Map<DynamicHierarchy, DynamicHierarchyDto>(dynamicHierarchy);
    }

    public async Task<DynamicHierarchyDto> UpdateAsync(Guid id, CreateDynamicHierarchyDto input)
    {   
        var dynamicHierarchy =
            await _dynamicHierarchyManager.UpdateAsync(id, input.Name, input.Description, input.Default);
        _dynamicHierarchyRepository.UpdateAsync(dynamicHierarchy);
        return ObjectMapper.Map<DynamicHierarchy, DynamicHierarchyDto>(dynamicHierarchy);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _dynamicHierarchyRepository.DeleteAsync(id);
    }

    public async Task<DynamicHierarchyDto> CreateDynamicHiererchyEntityAsync(Guid id,CreateDynamicHiererchyEntityDto input)
    {
        var dynamicHierarchy = await _dynamicHierarchyManager.AddEntityAsync(id,
            input.DynamicSourceEntityId,
            input.DynamicTargetEntityId,
            input.DisplayFields
        );
        return ObjectMapper.Map<DynamicHierarchy, DynamicHierarchyDto>(dynamicHierarchy);
    }

    public async Task<List<DynamicHierarchyEntityDto>> GetDynamicHierarchyEntitiesAsync(Guid hierarchyId)
    {
        var dynamicHierarchy = await _dynamicHierarchyRepository.GetByIdIncludeEntitiesAsync(hierarchyId);
        return ObjectMapper.Map<List<DynamicHierarchyEntities>, List<DynamicHierarchyEntityDto>>(dynamicHierarchy.Entities.ToList());
    }

    public async Task<DynamicHierarchyDto> UpdateDynamicHiererchyEntityAsync(Guid id,
        UpdateDynamicHierarchyEntityDto input)
    {
        var dynamicHierarchy = await _dynamicHierarchyManager.UpdateEntityAsync(id, input.Id,
            input.DynamicSourceEntityId, input.DynamicTargetEntityId, input.DisplayFields);
        _dynamicHierarchyRepository.UpdateAsync(dynamicHierarchy);
        return ObjectMapper.Map<DynamicHierarchy, DynamicHierarchyDto>(dynamicHierarchy);
    }

    public async Task DeleteDynamicHierarchyEntityAsync(Guid id, Guid entityId)
    {
        await _dynamicHierarchyManager.DeleteEntityAsync(id, entityId);
    }

    public async Task DeleteAllHierarchyEntityAsync(Guid id)
    {
        var dynamicHierarchy = await _dynamicHierarchyRepository.GetByIdIncludeEntitiesAsync(id);
        var ids = dynamicHierarchy.Entities.Select(e => e.Id).ToList();
        foreach (var idx in ids)
        {
            await DeleteDynamicHierarchyEntityAsync(id, idx);
        }
    }
}