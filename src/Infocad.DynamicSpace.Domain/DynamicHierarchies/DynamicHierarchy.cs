using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Infocad.DynamicSpace.DynamicHierarchies;

public class DynamicHierarchy : AuditedAggregateRoot<Guid>, IMultiTenant
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool Default { get; set; }
    public ICollection<DynamicHierarchyEntities> Entities { get; set; } = new List<DynamicHierarchyEntities>();

    internal DynamicHierarchy CreateDynamicHierarchyEntity(Guid? sourceId, Guid targetId, string displayFields)
    {
        Check.NotNullOrWhiteSpace(displayFields, nameof(displayFields));
        Check.Length(displayFields, nameof(displayFields), DynamicHierarchyConsts.MaxFieldsLenght);

        this.Entities.Add(new DynamicHierarchyEntities()
        {
            DynamicHierarchyId = this.Id,
            DynamicSourceEntityId = sourceId,
            DynamicTargetEntityId = targetId,
            DisplayFields = displayFields
        });

        return this;
    }

    internal DynamicHierarchy UpdateDynamicHierarchyEntity(Guid idEntity, Guid? sourceId, Guid targetId,
        string displayFields)
    {
        Check.NotNull(idEntity, nameof(idEntity));
        Check.NotNullOrWhiteSpace(displayFields, nameof(displayFields));
        Check.Length(displayFields, nameof(displayFields), DynamicHierarchyConsts.MaxFieldsLenght);

        var entity = this.Entities
                         .FirstOrDefault(e => e.Id == idEntity)
                     ?? throw new DynamicHierarchyEntityNotFoundException(idEntity);

        entity.DisplayFields = displayFields;

        return this;
    }

    internal void DeleteDynamicHierarchyEntity(Guid idEntity)
    {
        var entity = this.Entities
                         .FirstOrDefault(e => e.Id == idEntity)
                     ?? throw new DynamicHierarchyEntityNotFoundException(idEntity);
        this.Entities.Remove(entity);
    }

    public Guid? TenantId { get; set; }
}