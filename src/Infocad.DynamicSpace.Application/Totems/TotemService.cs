using Infocad.DynamicSpace.Permissions;
using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Infocad.DynamicSpace.Totems
{
    public class TotemService : CrudAppService<
        Totem,
        TotemDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateTotemDto>, ITotemService
    {
        public TotemService(ITotemRepository repository) : base(repository)
        {
            GetPolicyName = DynamicSpacePermissions.Totem.Default;
            GetListPolicyName = DynamicSpacePermissions.Totem.Default;
            CreatePolicyName = DynamicSpacePermissions.Totem.Create;
            UpdatePolicyName = DynamicSpacePermissions.Totem.Edit;
            DeletePolicyName = DynamicSpacePermissions.Totem.Delete;
        }
    }
}
