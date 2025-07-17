using Infocad.DynamicSpace.DynamicAttributes;
using System.ComponentModel.DataAnnotations;

namespace Infocad.DynamicSpace.DynamicFormats
{
    public class CreateDynamicFormatDto
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }

        [Required]
        public DynamicAttributeType AttributeType { get; set; }

        public string? FormatPattern { get; set; }
    }
}
