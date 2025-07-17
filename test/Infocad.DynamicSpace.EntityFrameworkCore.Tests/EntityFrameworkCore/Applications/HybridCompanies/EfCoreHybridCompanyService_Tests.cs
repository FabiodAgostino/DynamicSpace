using Infocad.DynamicSpace.HybridCompanies;
using Xunit;

namespace Infocad.DynamicSpace.EntityFrameworkCore.Applications.HybridCompanies
{
    [Collection(DynamicSpaceTestConsts.CollectionDefinitionName)]
    public class EfCoreHybridCompanyService_Tests : HybridCompanyService_Tests<DynamicSpaceEntityFrameworkCoreTestModule>
    {

    }
}
