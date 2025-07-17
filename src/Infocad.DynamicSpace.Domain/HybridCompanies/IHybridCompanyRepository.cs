using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Infocad.DynamicSpace.HybridCompanies
{
    public interface IHybridCompanyRepository : IRepository<HybridCompany, Guid>
    {
        Task<List<HybridCompany>> GetListByEntityAsync(Guid? entityId, int skipCount, int maxResultCount, string sorting, string filter = null);
    }
}
