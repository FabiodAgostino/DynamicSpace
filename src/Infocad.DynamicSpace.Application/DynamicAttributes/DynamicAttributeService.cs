using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infocad.DynamicSpace.DynamicAttirbutes;
using Infocad.DynamicSpace.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Validation;

namespace Infocad.DynamicSpace.DynamicAttributes;

public class DynamicAttributeService: CrudAppService<
    DynamicAttribute,
    DynamicAttributeDto,
    Guid,
    PagedAndSortedResultRequestDto,
    CreateDynamicAttributeDto>, IDynamicAttributeService
{
    private readonly IDynamicAttributeRepository _repository;

    public DynamicAttributeService(IDynamicAttributeRepository repository) :base(repository)
    {
        GetPolicyName = DynamicSpacePermissions.DynamicAttribute.Default;
        GetListPolicyName = DynamicSpacePermissions.DynamicAttribute.Default;
        CreatePolicyName = DynamicSpacePermissions.DynamicAttribute.Create;
        UpdatePolicyName = DynamicSpacePermissions.DynamicAttribute.Edit;
        DeletePolicyName = DynamicSpacePermissions.DynamicAttribute.Delete;
        
        _repository = repository;
    }

    public async Task<List<DynamicAttributeDto>> GetListByGuids(List<Guid> attributeIds)
    {
        var attributes = await _repository.GetListByGuidsAsync(attributeIds);
        return ObjectMapper.Map<List<DynamicAttribute>, List<DynamicAttributeDto>>(attributes);
    }
    
    public override async Task<DynamicAttributeDto> CreateAsync(CreateDynamicAttributeDto input)
    {
        await CheckCreatePolicyAsync();
    
        var entity = await MapToEntityAsync(input);
    
        TryToSetTenantId(entity);
        
        if(entity.Type == DynamicAttributeType.Navigation)
        {
            if (string.IsNullOrEmpty(entity.NavigationSettings))
            {
                throw new AbpValidationException("Navigation settings must be provided for navigation attributes.");
            }
        }
    
        await Repository.InsertAsync(entity, autoSave: true);
    
        return await MapToGetOutputDtoAsync(entity);
    }
    public override async Task<DynamicAttributeDto> UpdateAsync(Guid id, CreateDynamicAttributeDto input)
    {
        await CheckUpdatePolicyAsync();
    
        var entity = await Repository.GetAsync(id);
    
        if (entity.Type == DynamicAttributeType.Navigation && string.IsNullOrEmpty(input.NavigationSettings))
        {
            throw new AbpValidationException("Navigation settings must be provided for navigation attributes.");
        }
    
        ObjectMapper.Map(input, entity);
        entity.NavigationSettings = input.NavigationSettings;
        await _repository.UpdateAsync(entity, autoSave:true);

        return await MapToGetOutputDtoAsync(entity);
    }
    
    public async Task<List<DynamicAttributeDto>> GetNavChildEntities(Guid parentId)
    {
        var attributes = await _repository.GetListAsync(a =>
                    a.Type == DynamicAttributeType.Navigation && a.NavigationSettings.Contains(parentId.ToString())
            );
        return ObjectMapper.Map<List<DynamicAttribute>, List<DynamicAttributeDto>>(attributes);
    }
}
