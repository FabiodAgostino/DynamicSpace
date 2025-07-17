using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Infocad.DynamicSpace.DynamicEntities;

public class DynamicEntityManager : DomainService
{
    private readonly IDynamicEntityRepository _dynamicEntityRepository;

    public DynamicEntityManager(IDynamicEntityRepository dynamicEntityRepository)
    {
        _dynamicEntityRepository = dynamicEntityRepository;
    }
    
    public async Task<DynamicEntity>  CreateAsync(string name, string description,Guid dynamicTypeId, bool IsHybrid, string? HybridTypeName)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name));

        var existingStamp = await _dynamicEntityRepository.FindByNameAsync(name);
        if (existingStamp != null)
        {
            throw new DynamicEntityAlreadyExistsException(name);
        }

        var entity = new DynamicEntity { DynamicTypeId = dynamicTypeId, Name = name, Description = description, IsHybrid = IsHybrid, HybridTypeName = HybridTypeName};
        return entity;
    }
    
    public async Task ChangeNameAsync(
        DynamicEntity dynamicEntity,
        string newName)
    {
        Check.NotNull(dynamicEntity, nameof(dynamicEntity));
        Check.NotNullOrWhiteSpace(newName, nameof(newName));

        var existingAuthor = await _dynamicEntityRepository.FindByNameAsync(newName);
        if (existingAuthor != null && existingAuthor.Id != dynamicEntity.Id)
        {
            throw new DynamicEntityAlreadyExistsException(newName);
        }

        dynamicEntity.ChangeName(newName);
    }

    
    public async Task AddAttributeAsync(
        DynamicEntity dynamicEntity,
        Guid dynamicAttributeId,
        int order,
        string label, Guid? dynamicFormatId, Guid? dynmicRuleId, Guid? dynamicControlId, string? uiControl = null, bool required = false)
    {
        Check.NotNull(dynamicEntity, nameof(dynamicEntity));
        Check.NotNullOrWhiteSpace(label, nameof(label));
        Check.NotNull(dynamicAttributeId, nameof(dynamicAttributeId));

        var existingAttribute = dynamicEntity.Attributes
            .FirstOrDefault(x => x.DynamicAttributeId == dynamicAttributeId);
        if (existingAttribute != null)
        {
            throw new DynamicEntityAttributeAlreadyExistsException(label);
        }

        dynamicEntity.CreateAttribute(dynamicAttributeId, order, label, dynamicFormatId, dynmicRuleId, dynamicControlId,uiControl, required);
    }

    public async Task ReplaceAllAttributesAsync(
    DynamicEntity dynamicEntity,
    List<DynamicEntityAttribute> newAttributes)
    {
        Check.NotNull(dynamicEntity, nameof(dynamicEntity));
        Check.NotNull(newAttributes, nameof(newAttributes));

        // Rimuovi tutti gli attributi esistenti
        dynamicEntity.Attributes.Clear();

        // Aggiungi tutti i nuovi attributi
        foreach (var attr in newAttributes)
        {
            await AddAttributeAsync(dynamicEntity, attr.DynamicAttributeId, attr.Order, attr.Label, attr.DynamicFormatId, attr.DynamicRuleId, attr.DynamicControlId,required:attr.Required);
        }
    }

    public async Task UpdateAttributeAsync(
      DynamicEntity dynamicEntity,
      Guid dynamicAttributeId,
      int order,
      string label, Guid? dynamicFormatId, Guid? dynamicRuleId, DynamicEntityAttribute existingAttribute, string uiControl, bool required, bool replace=false)
    {
        Check.NotNull(dynamicEntity, nameof(dynamicEntity));
        Check.NotNullOrWhiteSpace(label, nameof(label));
        Check.NotNull(dynamicAttributeId, nameof(dynamicAttributeId));
        dynamicEntity.UpdateAttribute(dynamicAttributeId, order, label, dynamicFormatId, dynamicRuleId, existingAttribute,uiControl, required , replace);
    }

}