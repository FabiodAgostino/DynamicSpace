using Infocad.DynamicSpace.Hybrid;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Infocad.DynamicSpace.HybridCompanies
{
    public interface IHybridCompanyService : IHybridService<HybridCompanyDto, GetHybridCompanyListDto>
    {
    }
}
