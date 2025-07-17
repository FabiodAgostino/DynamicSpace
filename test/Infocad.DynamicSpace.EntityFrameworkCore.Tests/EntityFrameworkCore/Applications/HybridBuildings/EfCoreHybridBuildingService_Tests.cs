using Infocad.DynamicSpace.HybridBuildings;
using Xunit;

namespace Infocad.DynamicSpace.EntityFrameworkCore.Applications.HybridBuildings
{
    [Collection(DynamicSpaceTestConsts.CollectionDefinitionName)]
    public class EfCoreHybridBuildingService_Tests : HybridBuildingService_Tests<DynamicSpaceEntityFrameworkCoreTestModule>
    {

    }
}
