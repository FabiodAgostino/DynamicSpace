using Infocad.DynamicSpace.DynamicEntityAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Infocad.DynamicSpace.DynamicEntities
{
    public class UpdateDynamicEntityDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public Guid DynamicTypeId { get; set; }
        public bool IsHybrid { get; set; }
        public string? HybridTypeName { get; set; }

        public List<CreateDynamicEntityAttributeDto> Attributes { get; set; } = new();

        public UpdateDynamicEntityDto Clone()
        {
            return (UpdateDynamicEntityDto)this.MemberwiseClone();
        }

    }
}
