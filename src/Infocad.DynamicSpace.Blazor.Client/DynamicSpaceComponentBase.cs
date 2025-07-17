using Infocad.DynamicSpace.Localization;
using Volo.Abp.AspNetCore.Components;

namespace Infocad.DynamicSpace.Blazor.Client;

public abstract class DynamicSpaceComponentBase : AbpComponentBase
{
    protected DynamicSpaceComponentBase()
    {
        LocalizationResource = typeof(DynamicSpaceResource);
    }
}
