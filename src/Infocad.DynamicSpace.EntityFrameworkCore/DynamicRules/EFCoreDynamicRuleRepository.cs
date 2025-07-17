using Infocad.DynamicSpace.EntityFrameworkCore;
using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Infocad.DynamicSpace.DynamicRules
{
    internal class EFCoreDynamicRuleRepository : EfCoreRepository<DynamicSpaceDbContext, DynamicRule, Guid>,
    IDynamicRuleRepository
    {
        public EFCoreDynamicRuleRepository(IDbContextProvider<DynamicSpaceDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}
