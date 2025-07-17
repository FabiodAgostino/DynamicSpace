using Volo.Abp.Application.Dtos;

namespace Infocad.DynamicSpace.HybridRooms
{
    public class GetHybridRoomListDto : PagedAndSortedResultRequestDto
    {
        public string? Filter { get; set; }
    }
}
