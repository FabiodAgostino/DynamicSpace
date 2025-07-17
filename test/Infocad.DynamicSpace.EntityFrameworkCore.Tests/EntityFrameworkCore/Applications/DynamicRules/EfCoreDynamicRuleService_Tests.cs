using Infocad.DynamicSpace.DynamicRules;
using Infocad.DynamicSpace.DynamicTypes;
using Xunit;

namespace Infocad.DynamicSpace.EntityFrameworkCore.Applications.DynamicRules
{
    [Collection(DynamicSpaceTestConsts.CollectionDefinitionName)]
    public class EfCoreDynamicRuleService_Tests : DynamicRuleService_Tests<DynamicSpaceEntityFrameworkCoreTestModule>
    {

    }
}
