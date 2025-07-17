using Infocad.DynamicSpace.Totems;
using Xunit;

namespace Infocad.DynamicSpace.EntityFrameworkCore.Applications.Totems
{
    [Collection(DynamicSpaceTestConsts.CollectionDefinitionName)]

    public class EfCoreTotemService_Tests : TotemService_Tests<DynamicSpaceEntityFrameworkCoreTestModule>
    {

    }
}
