using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Infocad.DynamicSpace.DynamicRules
{
    public class DynamicRuleService : CrudAppService<
    DynamicRule,
    DynamicRuleDto,
    Guid,
    PagedAndSortedResultRequestDto,
    CreateDynamicRuleDto, UpdateDynamicRuleDto>, IDynamicRuleService
    {
        public DynamicRuleService(IDynamicRuleRepository repository) : base(repository)
        {
        }
    }
}
