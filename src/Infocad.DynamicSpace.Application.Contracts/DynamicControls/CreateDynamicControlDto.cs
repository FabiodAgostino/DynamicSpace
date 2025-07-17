using Infocad.DynamicSpace.DynamicAttributes;
using System.ComponentModel.DataAnnotations;

namespace Infocad.DynamicSpace.DynamicControls
{
    public class CreateDynamicControlDto
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
