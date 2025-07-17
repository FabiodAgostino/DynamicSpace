using Infocad.DynamicSpace.Localization;
using Infocad.DynamicSpace.Permissions;
using System;
using System.Threading.Tasks;
using Volo.Abp.Localization;

namespace Infocad.DynamicSpace.Blazor.Client.Pages
{
    public partial class Totems
    {
        public Totems()
        {
            LocalizationResource = typeof(DynamicSpaceResource);

            CreatePolicyName = DynamicSpacePermissions.Totem.Create;
            UpdatePolicyName = DynamicSpacePermissions.Totem.Edit;
            DeletePolicyName = DynamicSpacePermissions.Totem.Delete;
        }

        protected override async Task OpenCreateModalAsync()
        {
            await base.OpenCreateModalAsync();

        }
    }
}
