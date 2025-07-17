using Volo.Abp.Application.Dtos;

namespace Infocad.DynamicSpace.HybridBuildings
{
    public class GetHybridBuildingListDto : PagedAndSortedResultRequestDto
    {
        public string? Filter { get; set; }
    }
}
