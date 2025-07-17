using Infocad.DynamicSpace.Samples;
using Xunit;

namespace Infocad.DynamicSpace.EntityFrameworkCore.Applications;

[Collection(DynamicSpaceTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<DynamicSpaceEntityFrameworkCoreTestModule>
{

}
