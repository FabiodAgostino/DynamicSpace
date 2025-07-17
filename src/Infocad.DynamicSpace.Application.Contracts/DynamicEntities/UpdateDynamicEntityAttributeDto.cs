using System;
using System.ComponentModel.DataAnnotations;

namespace Infocad.DynamicSpace.DynamicEntityAttributes
{
    public class UpdateDynamicEntityAttributeDto
    {
        [Required]
        public Guid Id { get; set; }
        
        [Required]
        public Guid DynamicEntityId { get; set; }
        
        [Required]
        public Guid DynamicAttributeId { get; set; }

        public int Order { get; set; }

        [StringLength(255)]
        public string Label { get; set; }
        [StringLength(255)]
        public string? UIControl { get; set; }

        public bool Required { get; set; }
    }
}
