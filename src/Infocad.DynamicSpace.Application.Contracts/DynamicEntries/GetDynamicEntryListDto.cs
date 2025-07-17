using Volo.Abp.Application.Dtos;

namespace Infocad.DynamicSpace.DynamicEntries;

public class GetDynamicEntryListDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}