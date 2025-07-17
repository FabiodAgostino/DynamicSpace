using Infocad.DynamicSpace.DynamicAttributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace Infocad.DynamicSpace.DynamicRules
{
    public class CreateDynamicRuleDto
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public string Rule { get; set; }

        [Required]
        public DynamicAttributeType AttributeType { get; set; }


        public CreateDynamicRuleDto()
        {
        }
    }
}
