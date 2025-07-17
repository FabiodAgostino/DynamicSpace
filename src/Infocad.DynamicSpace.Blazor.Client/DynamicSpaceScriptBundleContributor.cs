using Volo.Abp.AspNetCore.Mvc.UI.Bundling;

namespace Infocad.DynamicSpace.Blazor.Client;

public class DynamicSpaceScriptBundleContributor : BundleContributor
{
    public override void ConfigureBundle(BundleConfigurationContext context)
    {
        context.Files.Add(new BundleFile("bootstrap-patch.js", true));
        context.Files.Add(new BundleFile("powerbi.js", true));

    }
}