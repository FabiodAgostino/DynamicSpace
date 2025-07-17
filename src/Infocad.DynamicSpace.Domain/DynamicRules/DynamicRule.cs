using Infocad.DynamicSpace.DynamicAttributes;
using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Infocad.DynamicSpace.DynamicRules
{
    public class DynamicRule : AuditedEntity<Guid>
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Rule { get; set; }
        public DynamicAttributeType AttributeType { get; set; }

        public DynamicRule()
        {
            Id = Guid.NewGuid();
        }

    }
}
