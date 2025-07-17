using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infocad.DynamicSpace.DynamicEntities;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Infocad.DynamicSpace.DynamicAttributes;

public interface IDynamicAttributeService : ICrudAppService<
    DynamicAttributeDto, Guid,PagedAndSortedResultRequestDto,
    CreateDynamicAttributeDto>
{
    Task<List<DynamicAttributeDto>> GetListByGuids(List<Guid> attributeIds);

    Task<List<DynamicAttributeDto>> GetNavChildEntities(Guid parentId);
}