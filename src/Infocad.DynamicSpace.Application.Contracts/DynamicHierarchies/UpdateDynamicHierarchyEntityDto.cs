using System;

namespace Infocad.DynamicSpace.DynamicHierarchies;

public class UpdateDynamicHierarchyEntityDto
{
        public Guid Id { get; set; }
        public Guid? DynamicSourceEntityId { get; set; }
        public Guid DynamicTargetEntityId { get; set; }
        public string DisplayFields { get; set; } = string.Empty;
}