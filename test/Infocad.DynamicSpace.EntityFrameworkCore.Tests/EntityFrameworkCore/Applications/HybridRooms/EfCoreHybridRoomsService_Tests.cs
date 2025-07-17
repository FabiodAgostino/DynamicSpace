using Infocad.DynamicSpace.HybridRooms;
using Xunit;

namespace Infocad.DynamicSpace.EntityFrameworkCore.Applications.HybridRooms
{
    [Collection(DynamicSpaceTestConsts.CollectionDefinitionName)]
    public class EfCoreHybridRoomsService_Tests : HybridRoomService_Tests<DynamicSpaceEntityFrameworkCoreTestModule>
    {

    }
}
