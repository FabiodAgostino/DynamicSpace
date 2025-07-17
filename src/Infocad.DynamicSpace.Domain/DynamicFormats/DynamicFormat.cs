using Infocad.DynamicSpace.DynamicAttributes;
using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Infocad.DynamicSpace.DynamicFormats
{
    public class DynamicFormat : AuditedEntity<Guid>
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public DynamicAttributeType AttributeType { get; set; }
        public string? FormatPattern { get; set; }

        public DynamicFormat()
        {
            Id = Guid.NewGuid();
        }

        public DynamicFormat(Guid id, string name, DynamicAttributeType attributeType, string? description = null, string? formatPattern = null)
        {
            Id = id;
            Name = name;
            AttributeType = attributeType;
            Description = description;
            FormatPattern = formatPattern;
        }

    }
}
