using Infocad.DynamicSpace.DynamicControls;
using Infocad.DynamicSpace.DynamicEntities;
using Xunit;

namespace Infocad.DynamicSpace.EntityFrameworkCore.Applications.DynamicControls
{
    [Collection(DynamicSpaceTestConsts.CollectionDefinitionName)]
    public class EfCoreDynamicControlService_Tests : DynamicControlService_Tests<DynamicSpaceEntityFrameworkCoreTestModule>
    {

    }
}
