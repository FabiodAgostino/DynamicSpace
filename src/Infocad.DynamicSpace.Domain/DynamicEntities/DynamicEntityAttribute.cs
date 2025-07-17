using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Infocad.DynamicSpace.DynamicEntities;

public class DynamicEntityAttribute : AuditedEntity<Guid>
{
    private DynamicEntityAttribute()
    {
        // This constructor is intentionally left empty to prevent instantiation without parameters.
    }
    
    public DynamicEntityAttribute(Guid dynamicAttributeId, int order, string label, string uicontrol, bool required, Guid? dynamicFormatId,
        Guid? dynamicRuleId) 
    {
        if (dynamicFormatId == new Guid())
            dynamicFormatId = null;
        if (dynamicRuleId == new Guid())
            dynamicRuleId = null;

        Id = Guid.NewGuid();
        DynamicAttributeId = dynamicAttributeId;
        Order = order;
        Label = label;
        DynamicFormatId = dynamicFormatId;
        DynamicRuleId = dynamicRuleId;
        UIControl = uicontrol;
        Required = required;
    }

    public DynamicEntityAttribute(Guid dynamicAttributeId, int order, string label, string uicontrol, bool required, Guid? dynamicFormatId,
        Guid? dynamicRuleId, Guid? dynamicControlId)
    {
        if (dynamicFormatId == new Guid())
            dynamicFormatId = null;
        if (dynamicRuleId == new Guid())
            dynamicRuleId = null;
        if (dynamicControlId == new Guid())
            dynamicControlId = null;

        Id = Guid.NewGuid();
        DynamicAttributeId = dynamicAttributeId;
        Order = order;
        Label = label;
        DynamicFormatId = dynamicFormatId;
        DynamicRuleId = dynamicRuleId;
        UIControl = uicontrol;
        Required = required;
        DynamicControlId = dynamicControlId;
    }

    public virtual Guid DynamicEntityId { get; private set; }
    public virtual Guid DynamicAttributeId { get; private set; }
    public virtual Guid? DynamicFormatId { get; set; }
    public virtual Guid? DynamicRuleId { get; set; }
    public virtual Guid? DynamicControlId { get; set; }


    public virtual int Order { get; set; }
    public string Label { get; set; }

    public string? UIControl { get; set; }

    public bool Required { get; set; }

    public void ChangeLabel(string inputLabel)
    {
        Check.NotNullOrWhiteSpace(inputLabel, nameof(inputLabel));
        Check.Length(inputLabel, nameof(inputLabel), DynamicEntityConsts.MaxLabelLength);
        Label = inputLabel;
    }

    public void ChangeOrder(int inputOrder)
    {
        Order = inputOrder;
    }

    public bool CheckEdit(DynamicEntityAttribute editAttribute)
    {
        return Label != editAttribute.Label || Order != editAttribute.Order || DynamicFormatId != editAttribute.DynamicFormatId;
    }


}