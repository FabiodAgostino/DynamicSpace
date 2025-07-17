using System;
using Infocad.DynamicSpace.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Infocad.DynamicSpace.DynamicFormats
{
    public class DynamicFormatService : CrudAppService<
    DynamicFormat,
    DynamicFormatDto,
    Guid,
    PagedAndSortedResultRequestDto,
    CreateDynamicFormatDto, UpdateDynamicFormatDto>, IDynamicFormatService
    {
        public DynamicFormatService(IDynamicFormatRepository repository) : base(repository)
        {
            // GetPolicyName = DynamicSpacePermissions.DynamicFormat.Default;
            // GetListPolicyName = DynamicSpacePermissions.DynamicFormat.Default;
            // CreatePolicyName = DynamicSpacePermissions.DynamicFormat.Create;
            // UpdatePolicyName = DynamicSpacePermissions.DynamicFormat.Edit;
            // DeletePolicyName = DynamicSpacePermissions.DynamicFormat.Delete;
        }
    }
}
