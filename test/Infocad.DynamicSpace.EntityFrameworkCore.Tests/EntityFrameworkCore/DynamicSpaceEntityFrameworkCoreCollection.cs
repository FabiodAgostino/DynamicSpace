using Xunit;

namespace Infocad.DynamicSpace.EntityFrameworkCore;

[CollectionDefinition(DynamicSpaceTestConsts.CollectionDefinitionName)]
public class DynamicSpaceEntityFrameworkCoreCollection : ICollectionFixture<DynamicSpaceEntityFrameworkCoreFixture>
{

}
