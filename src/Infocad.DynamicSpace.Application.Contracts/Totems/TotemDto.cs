using System;
using Volo.Abp.Application.Dtos;

namespace Infocad.DynamicSpace.Totems
{
    public class TotemDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
