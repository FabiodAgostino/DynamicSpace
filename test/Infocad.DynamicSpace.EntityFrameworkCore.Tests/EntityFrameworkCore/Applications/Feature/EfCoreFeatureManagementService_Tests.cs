using Infocad.DynamicSpace.Feature;
using Xunit;

namespace Infocad.DynamicSpace.EntityFrameworkCore.Applications.Feature
{
    [Collection(DynamicSpaceTestConsts.CollectionDefinitionName)]
    public class EfCoreFeatureManagementService_Tests : FeatureManagementService_Tests<DynamicSpaceEntityFrameworkCoreTestModule>
    {

    }
}
