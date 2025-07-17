using Microsoft.Extensions.Localization;
using Infocad.DynamicSpace.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Infocad.DynamicSpace.Blazor;

[Dependency(ReplaceServices = true)]
public class DynamicSpaceBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<DynamicSpaceResource> _localizer;

    public DynamicSpaceBrandingProvider(IStringLocalizer<DynamicSpaceResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
