using Volo.Abp.Application.Dtos;

namespace Infocad.DynamicSpace.DynamicHierarchies;

public class GetDyanmicHierarchyListDto: PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}