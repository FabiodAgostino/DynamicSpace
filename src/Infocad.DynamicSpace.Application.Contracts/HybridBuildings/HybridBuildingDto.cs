using System;
using Volo.Abp.Application.Dtos;

namespace Infocad.DynamicSpace.HybridBuildings
{
    public class HybridBuildingDto : ExtensibleEntityDto<Guid>
    {
        public Guid? DynamicEntityId
        {
            get => _dynamicEntityId; set
            {
                if (value == Guid.Empty)
                    _dynamicEntityId = null;
                else
                    _dynamicEntityId = value;
            }
        }
        private Guid? _dynamicEntityId { get; set; }

        public float X { get; set; }
        public float Y { get; set; }
        public string Name { get; set; }
    }
}
