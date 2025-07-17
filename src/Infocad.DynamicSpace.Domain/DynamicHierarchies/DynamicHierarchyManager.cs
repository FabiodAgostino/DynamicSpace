using System;
using System.Threading.Tasks;
using Infocad.DynamicSpace.DynamicEntities;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Infocad.DynamicSpace.DynamicHierarchies;

public class DynamicHierarchyManager : DomainService
{
    private readonly IDynamicHierarchyRepository _dynamicHierarchyRepository;

    public DynamicHierarchyManager(IDynamicHierarchyRepository dynamicHierarchyRepository)
    {
        _dynamicHierarchyRepository = dynamicHierarchyRepository;
    }

    public async Task<DynamicHierarchy> CreateAsync(string name, string description, bool defaultHierarchy)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name));

        var existingHierarchy = await _dynamicHierarchyRepository.FindByNameAsync(name);
        if (existingHierarchy != null)
        {
            throw new DynamicHierarchyAlreadyExistsException(name);
        }

        var hierarchy = new DynamicHierarchy { Name = name, Description = description, Default = defaultHierarchy };
        return hierarchy;
    }

    public async Task<DynamicHierarchy> UpdateAsync(Guid id, string newName, string newDescription,
        bool newDefault)
    {
        Check.NotNull(id, nameof(id));
        Check.NotNullOrWhiteSpace(newName, nameof(newName));
        
        var dynamicHierarchy = await _dynamicHierarchyRepository.GetAsync(id);
        if (dynamicHierarchy == null)
        {
            throw new DynamicHierarchyNotFoundException(id);
        }

        var existingHierarchy = await _dynamicHierarchyRepository.FindByNameAsync(newName);
        if (existingHierarchy != null && existingHierarchy.Id != dynamicHierarchy.Id)
        {
            throw new DynamicHierarchyAlreadyExistsException(newName);
        }

        dynamicHierarchy.Name = newName;
        dynamicHierarchy.Description = newDescription;
        dynamicHierarchy.Default = newDefault;

        return dynamicHierarchy;
    }
    
    public async Task<DynamicHierarchy> AddEntityAsync(
        Guid id,
        Guid? sourceId,
        Guid targetId,
        string dispalyFields)
    {
        Check.NotNull(id, nameof(id));
        Check.NotNullOrWhiteSpace(dispalyFields, nameof(dispalyFields));
        Check.NotNull(targetId, nameof(targetId));
    
        var hierarchy = await _dynamicHierarchyRepository.GetAsync(id);
        if (hierarchy == null)
        {
            throw new DynamicHierarchyNotFoundException(id);
        }
        hierarchy.CreateDynamicHierarchyEntity(sourceId, targetId, dispalyFields);
        return hierarchy;
    }

    public async Task<DynamicHierarchy> UpdateEntityAsync( Guid id,Guid idEntity,
        Guid? sourceId,
        Guid targetId,
        string dispalyFields)
    {
        Check.NotNull(id, nameof(id));
        Check.NotNull(idEntity, nameof(idEntity));
        Check.NotNullOrWhiteSpace(dispalyFields, nameof(dispalyFields));
        Check.NotNull(targetId, nameof(targetId));
    
        var hierarchy = await _dynamicHierarchyRepository.GetByIdIncludeEntitiesAsync(id);
        if (hierarchy == null)
        {
            throw new DynamicHierarchyNotFoundException(id);
        }
        hierarchy.UpdateDynamicHierarchyEntity(idEntity, sourceId, targetId, dispalyFields);
        return hierarchy;
    }

    public async Task DeleteEntityAsync(Guid id, Guid entityId)
    {
        Check.NotNull(id, nameof(id));
        Check.NotNull(entityId, nameof(entityId));

        var hierarchy = await _dynamicHierarchyRepository.GetByIdIncludeEntitiesAsync(id);
        if (hierarchy == null)
        {
            throw new DynamicHierarchyNotFoundException(id);
        }
        hierarchy.DeleteDynamicHierarchyEntity(entityId);
        await _dynamicHierarchyRepository.UpdateAsync(hierarchy);
    }
}