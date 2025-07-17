using Infocad.DynamicSpace.DynamicEntities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Infocad.DynamicSpace.DynamicEntityAttributes
{
    public class CreateDynamicEntityAttributeDto
    {
        [Required] public Guid DynamicEntityId { get; set; }

        [Required] public Guid DynamicAttributeId { get; set; }

        public Guid? DynamicFormatId { get => _dynamicFormatId; set => _dynamicFormatId = value == Guid.Empty ? null : value; }
        private Guid? _dynamicFormatId { get; set; }

        public Guid? DynamicRuleId { get => _dynamicRuleId; set => _dynamicRuleId = value == Guid.Empty ? null : value; }
        private Guid? _dynamicRuleId { get; set; }

        public Guid? DynamicControlId { get => _dynamicControlId; set => _dynamicControlId = value == Guid.Empty ? null : value; }
        private Guid? _dynamicControlId { get; set; }


        public int Order { get; set; }

        public string Label { get; set; }

        public bool IsNew { get; set; }

        public string? UIControl { get; set; }

        public bool Required { get; set; } = false;

        public CreateDynamicEntityAttributeDto Clone()
        {
            return (CreateDynamicEntityAttributeDto)this.MemberwiseClone();
        }
    }
}