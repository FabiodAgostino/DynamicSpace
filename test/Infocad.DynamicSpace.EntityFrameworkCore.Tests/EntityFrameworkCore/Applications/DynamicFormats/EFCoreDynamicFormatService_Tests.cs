using Infocad.DynamicSpace.DynamicEntries;
using Infocad.DynamicSpace.DynamicFormats;
using Xunit;

namespace Infocad.DynamicSpace.EntityFrameworkCore.Applications.DynamicFormats
{
    [Collection(DynamicSpaceTestConsts.CollectionDefinitionName)]
    public class EFCoreDynamicFormatService_Tests : DynamicFormatService_Tests<DynamicSpaceEntityFrameworkCoreTestModule>
    {

    }
}
