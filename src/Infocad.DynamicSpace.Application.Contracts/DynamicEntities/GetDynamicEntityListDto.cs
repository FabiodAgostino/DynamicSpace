using Volo.Abp.Application.Dtos;

namespace Infocad.DynamicSpace.DynamicEntities;

public class GetDynamicEntityListDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}