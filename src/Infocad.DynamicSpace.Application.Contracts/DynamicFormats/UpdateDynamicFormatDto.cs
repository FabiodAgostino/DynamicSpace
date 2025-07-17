using Infocad.DynamicSpace.DynamicAttributes;
using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Infocad.DynamicSpace.DynamicFormats
{
    public class UpdateDynamicFormatDto : EntityDto<Guid>
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }

        [Required]
        public DynamicAttributeType AttributeType { get; set; }

        public string? FormatPattern { get; set; }
    }
}
