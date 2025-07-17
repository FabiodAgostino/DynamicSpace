using System;
using Volo.Abp.Application.Dtos;

namespace Infocad.DynamicSpace.HybridRooms
{
    public class HybridRoomDto : ExtensibleEntityDto<Guid>
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
        public string Name { get; set; }
        public int Capacity { get; set; }
        public string Description { get; set; }

        public HybridRoomDto Clone()
        {
            return (HybridRoomDto)MemberwiseClone();
        }
    }
}
