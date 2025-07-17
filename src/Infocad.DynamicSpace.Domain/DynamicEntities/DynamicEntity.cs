using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Infocad.DynamicSpace.DynamicEntities;

public class DynamicEntity : AuditedAggregateRoot<Guid>
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public Guid DynamicTypeId { get; set; }
    public bool IsHybrid { get; set; }
    public string? HybridTypeName { get; set; }
    public ICollection<DynamicEntityAttribute> Attributes { get; set; } = new List<DynamicEntityAttribute>();

    internal DynamicEntity ChangeName(string name)
    {
        SetName(name);
        return this;
    }

    private void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(
            name,
            nameof(name),
            maxLength: DynamicEntityConsts.MaxNameLenght
        );
    }

    internal DynamicEntity CreateAttribute(Guid dynamicAttributeId, int order, string label, Guid? dynamicFormatId, Guid? dynamicRuleId, Guid? dynamicControlId,string? uiControl, bool required = false)
    {
        Check.NotNullOrWhiteSpace(label, nameof(label));
        Check.NotNull(dynamicAttributeId, nameof(dynamicAttributeId));
        Check.Length(label, nameof(label), DynamicEntityConsts.MaxLabelLength);

        Attributes.Add(new DynamicEntityAttribute(dynamicAttributeId, order, label,uiControl,required, dynamicFormatId,dynamicRuleId, dynamicControlId));

        return this;
    }

    internal DynamicEntity UpdateAttribute(Guid dynamicAttributeId, int order, string label, Guid? dynamicFormatId, Guid? dynamicRuleId, DynamicEntityAttribute existingAttribute, string uiControl, bool required ,bool replace)
    {
        Check.NotNullOrWhiteSpace(label, nameof(label));
        Check.NotNull(dynamicAttributeId, nameof(dynamicAttributeId));
        Check.Length(label, nameof(label), DynamicEntityConsts.MaxLabelLength);

        if (replace)
        {
            Attributes.Remove(existingAttribute);
            Attributes.Add(new DynamicEntityAttribute(
                dynamicAttributeId,
                order,
                label,
                uiControl,
                required,
                dynamicFormatId, dynamicRuleId));
        }
        else
        {
            existingAttribute.Label = label;
            existingAttribute.Order = order;
            existingAttribute.UIControl = uiControl;
            existingAttribute.Required = required;
            existingAttribute.DynamicFormatId = dynamicFormatId;
            existingAttribute.DynamicRuleId = dynamicRuleId;
        }
        return this;
    }

    public bool CheckEdit(string name, string description, Guid dynamicTypeId, string hybridTypeName)
    {
        return Name != name || Description != description || DynamicTypeId != dynamicTypeId || HybridTypeName!=hybridTypeName;
    }
    public void RemoveAttribute(DynamicEntityAttribute attribute)
    {
        Attributes.Remove(attribute);
    }

    public List<DynamicEntityAttribute> CheckEditAttributes(List<DynamicEntityAttribute> attributes)
    {
        var list = new List<DynamicEntityAttribute>();
        var sortedBase = Attributes
            .OrderBy(x => x.DynamicAttributeId)
            .ThenBy(x => x.Order)
            .ThenBy(x => x.Label)
            .ThenBy(x => x.DynamicFormatId.HasValue ? x.DynamicFormatId : Guid.Empty)
            .ThenBy(x => x.DynamicRuleId.HasValue ? x.DynamicRuleId : Guid.Empty)
            .ToList();

        var sortedEdit = attributes
            .OrderBy(x => x.DynamicAttributeId)
            .ThenBy(x => x.Order)
            .ThenBy(x => x.Label)
            .ThenBy(x => x.DynamicFormatId.HasValue ? x.DynamicFormatId : Guid.Empty)
            .ThenBy(x => x.DynamicRuleId.HasValue ? x.DynamicRuleId: Guid.Empty)
            .ToList();

        for(int i=0; i<sortedBase.Count; i++)
        {
            if (sortedBase[i].CheckEdit(sortedEdit[i]))
                list.Add(sortedEdit[i]);
        }
        return list;
    }
}