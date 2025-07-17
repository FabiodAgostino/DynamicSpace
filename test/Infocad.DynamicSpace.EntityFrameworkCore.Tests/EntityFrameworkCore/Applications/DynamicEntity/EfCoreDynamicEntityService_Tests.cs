using Infocad.DynamicSpace.DynamicEntities;
using Xunit;

namespace Infocad.DynamicSpace.EntityFrameworkCore.Applications.DynamicEntity
{
    [Collection(DynamicSpaceTestConsts.CollectionDefinitionName)]
    public class EfCoreDynamicEntityService_Tests : DynamicEntityService_Test<DynamicSpaceEntityFrameworkCoreTestModule>
    {

    }

}
