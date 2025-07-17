using Infocad.DynamicSpace.Samples;
using Xunit;

namespace Infocad.DynamicSpace.EntityFrameworkCore.Domains;

[Collection(DynamicSpaceTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<DynamicSpaceEntityFrameworkCoreTestModule>
{

}
