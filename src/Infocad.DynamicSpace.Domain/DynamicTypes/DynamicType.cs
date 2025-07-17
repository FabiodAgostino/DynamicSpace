using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Infocad.DynamicSpace.DynamicTypes;

public class DynamicType : AuditedEntity<Guid>
{
    public string Name { get; set; }
    public string? Description { get; set; }
}