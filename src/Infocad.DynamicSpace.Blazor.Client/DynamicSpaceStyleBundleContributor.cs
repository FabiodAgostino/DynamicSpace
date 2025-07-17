using Volo.Abp.AspNetCore.Mvc.UI.Bundling;

namespace Infocad.DynamicSpace.Blazor.Client;

public class DynamicSpaceStyleBundleContributor : BundleContributor
{
    public override void ConfigureBundle(BundleConfigurationContext context)
    {
        context.Files.Add(new BundleFile("main.css", true));
    }
}
