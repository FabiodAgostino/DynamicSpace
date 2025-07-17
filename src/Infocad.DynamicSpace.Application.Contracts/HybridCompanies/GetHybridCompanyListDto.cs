using Volo.Abp.Application.Dtos;

namespace Infocad.DynamicSpace.HybridCompanies
{
    public class GetHybridCompanyListDto : PagedAndSortedResultRequestDto
    {
        public string? Filter { get; set; }
    }
}
