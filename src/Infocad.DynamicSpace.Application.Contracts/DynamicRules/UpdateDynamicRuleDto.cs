using Infocad.DynamicSpace.DynamicAttributes;
using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Infocad.DynamicSpace.DynamicRules
{
    public class UpdateDynamicRuleDto : EntityDto<Guid>
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }

        [Required]
        public string Rule { get; set; }

        [Required]
        public DynamicAttributeType AttributeType { get; set; }


    }
}
