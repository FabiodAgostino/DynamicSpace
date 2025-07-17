using Infocad.DynamicSpace.FileManagement;
using Xunit;

namespace Infocad.DynamicSpace.EntityFrameworkCore.Applications.FileManagement
{
    [Collection(DynamicSpaceTestConsts.CollectionDefinitionName)]
    public class EfCoreFileManagementService_Tests : FileManagementAppService_Tests<DynamicSpaceEntityFrameworkCoreTestModule>
    {

    }
}
