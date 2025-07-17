using Infocad.DynamicSpace.Localization;
using Volo.Abp.Application.Services;

namespace Infocad.DynamicSpace;

/* Inherit your application services from this class.
 */
public abstract class DynamicSpaceAppService : ApplicationService
{
    protected DynamicSpaceAppService()
    {
        LocalizationResource = typeof(DynamicSpaceResource);
    }
}
