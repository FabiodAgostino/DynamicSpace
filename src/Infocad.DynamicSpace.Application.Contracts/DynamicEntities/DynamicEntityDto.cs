using Infocad.DynamicSpace.DynamicAttributes;
using Infocad.DynamicSpace.DynamicEntityAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Infocad.DynamicSpace.DynamicEntities
{
    public class DynamicEntityDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public Guid DynamicTypeId { get; set; }
        public bool IsHybrid { get; set; }
        public string? HybridTypeName { get; set; }
        public List<DynamicEntityAttributeDto> Attributes { get; set; }

    }
}
