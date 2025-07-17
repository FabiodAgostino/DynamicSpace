using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Infocad.DynamicSpace.DynamicRules
{
    public interface IDynamicRuleService : ICrudAppService<
    DynamicRuleDto, Guid, PagedAndSortedResultRequestDto,
    CreateDynamicRuleDto, UpdateDynamicRuleDto>
    {
    }
}
