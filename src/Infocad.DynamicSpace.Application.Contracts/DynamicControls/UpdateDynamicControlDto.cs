using Infocad.DynamicSpace.DynamicAttributes;
using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Infocad.DynamicSpace.DynamicControls
{
    public class UpdateDynamicControlDto
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public DynamicAttributeType Type { get; set; }
        [Required]
        public string ComponentType { get; set; }

    }
}
