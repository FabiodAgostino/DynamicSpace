using Infocad.DynamicSpace.DynamicAttributes;
using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;

namespace Infocad.DynamicSpace.DynamicControls
{
    public class DynamicControl : AuditedEntity<Guid>
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public DynamicAttributeType Type { get; set; }
        [Required]
        public string ComponentType { get; set; }
    }
}
