using Infocad.DynamicSpace.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Infocad.DynamicSpace.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class DynamicSpaceController : AbpControllerBase
{
    protected DynamicSpaceController()
    {
        LocalizationResource = typeof(DynamicSpaceResource);
    }
}
