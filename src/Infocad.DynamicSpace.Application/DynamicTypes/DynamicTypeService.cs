using System;
using Infocad.DynamicSpace.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Infocad.DynamicSpace.DynamicTypes;

public class DynamicTypeService : CrudAppService<
DynamicType,
DynamicTypeDto,
Guid,
PagedAndSortedResultRequestDto,
CreateDynamicTypeDto>, IDynamicTypeService
{
    public DynamicTypeService(IDynamicTypeRepository repository) : base(repository)
    {
        GetPolicyName = DynamicSpacePermissions.DynamicType.Default;
        GetListPolicyName = DynamicSpacePermissions.DynamicType.Default;
        CreatePolicyName = DynamicSpacePermissions.DynamicType.Create;
        UpdatePolicyName = DynamicSpacePermissions.DynamicType.Edit;
        DeletePolicyName = DynamicSpacePermissions.DynamicType.Delete;
    }
}