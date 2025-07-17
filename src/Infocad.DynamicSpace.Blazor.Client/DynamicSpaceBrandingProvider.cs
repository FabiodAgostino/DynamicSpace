using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;
using Microsoft.Extensions.Localization;
using Infocad.DynamicSpace.Localization;

namespace Infocad.DynamicSpace.Blazor.Client;

public class DynamicSpaceBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<DynamicSpaceResource> _localizer;

    public DynamicSpaceBrandingProvider(IStringLocalizer<DynamicSpaceResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
