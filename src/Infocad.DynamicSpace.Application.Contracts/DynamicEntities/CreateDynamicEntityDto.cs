using Infocad.DynamicSpace.DynamicAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Infocad.DynamicSpace.DynamicEntityAttributes;

namespace Infocad.DynamicSpace.DynamicEntities
{
    public class CreateDynamicEntityDto
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public Guid DynamicTypeId { get; set; }
        public Guid? DynamicFormatId { get; set; }

        public bool IsHybrid { get; set; }
        public string? HybridTypeName { get; set; }
        public List<CreateDynamicEntityAttributeDto> Attributes { get; set; } = new();

        public CreateDynamicEntityDto Clone()
        {
            return (CreateDynamicEntityDto)this.MemberwiseClone();
        }
    }
}
