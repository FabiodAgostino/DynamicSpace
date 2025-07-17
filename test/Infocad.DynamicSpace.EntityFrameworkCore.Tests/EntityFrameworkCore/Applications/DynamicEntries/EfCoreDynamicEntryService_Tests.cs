using Infocad.DynamicSpace.DynamicEntries;
using Xunit;

namespace Infocad.DynamicSpace.EntityFrameworkCore.Applications.DynamicEntries;

[Collection(DynamicSpaceTestConsts.CollectionDefinitionName)]
public class EfCoreDynamicEntryService_Tests : DynamicEntryService_Tests<DynamicSpaceEntityFrameworkCoreTestModule>
{
    
}
