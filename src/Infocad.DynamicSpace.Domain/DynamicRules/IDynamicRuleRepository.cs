using System;
using Volo.Abp.Domain.Repositories;

namespace Infocad.DynamicSpace.DynamicRules
{
    public interface IDynamicRuleRepository : IRepository<DynamicRule, Guid>
    {

    }
}
