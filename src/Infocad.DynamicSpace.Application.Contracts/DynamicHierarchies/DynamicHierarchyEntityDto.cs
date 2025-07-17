using System;
using Volo.Abp.Application.Dtos;

namespace Infocad.DynamicSpace.DynamicHierarchies;

public class DynamicHierarchyEntityDto : EntityDto<Guid>
{
    public Guid DynamicHierarchyId { get; set; }
    public Guid? DynamicSourceEntityId { get; set; } //padre
    public Guid DynamicTargetEntityId { get; set; } //figlia o se stessa se padre
    public string DisplayFields { get; set; }
}