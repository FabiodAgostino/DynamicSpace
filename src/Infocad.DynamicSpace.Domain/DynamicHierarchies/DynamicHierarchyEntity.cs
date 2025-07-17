using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Infocad.DynamicSpace.DynamicHierarchies;

public class DynamicHierarchyEntities : AuditedEntity<Guid>  
{
    public Guid DynamicHierarchyId { get; set; }
    public Guid? DynamicSourceEntityId { get; set; } //Padre
    public Guid DynamicTargetEntityId { get; set; } //Figlio
    public string DisplayFields { get; set; }
    public int Order { get; set; }
}