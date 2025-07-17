using Infocad.DynamicSpace.DynamicAttributes;
using System;
using Volo.Abp.Application.Dtos;

namespace Infocad.DynamicSpace.DynamicControls
{
    public class DynamicControlDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public DynamicAttributeType Type { get; set; }
        public string ComponentType { get; set; }

        public DynamicControlDto()
        {
            Id = Guid.NewGuid();
        }
    }
}
