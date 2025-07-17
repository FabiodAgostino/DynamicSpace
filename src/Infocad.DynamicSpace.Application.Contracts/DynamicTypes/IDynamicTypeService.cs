using System;
using Infocad.DynamicSpace.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Infocad.DynamicSpace.DynamicTypes;

public interface IDynamicTypeService : ICrudAppService<
DynamicTypeDto, Guid,PagedAndSortedResultRequestDto,
CreateDynamicTypeDto>
{
    
}