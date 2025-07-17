using System;
using System.Collections.Generic;
using Infocad.DynamicSpace.DynamicAttributes;
using Infocad.DynamicSpace.DynamicEntities;
using Volo.Abp.Domain.Entities.Auditing;

namespace Infocad.DynamicSpace.DynamicAttirbutes;

public class DynamicAttribute : AuditedEntity<Guid>
{

    public string Name { get; set; }
    public string? Description { get; set; }
    public DynamicAttributeType Type { get; set; }
    
    public string? NavigationSettings { get; set; }
}