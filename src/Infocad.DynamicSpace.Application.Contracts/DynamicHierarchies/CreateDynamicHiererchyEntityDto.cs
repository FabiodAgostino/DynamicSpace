using System;

namespace Infocad.DynamicSpace.DynamicHierarchies;

public class CreateDynamicHiererchyEntityDto
{
    public Guid? DynamicSourceEntityId { get; set; }
    public Guid DynamicTargetEntityId { get; set; }
    public string DisplayFields { get; set; }
}