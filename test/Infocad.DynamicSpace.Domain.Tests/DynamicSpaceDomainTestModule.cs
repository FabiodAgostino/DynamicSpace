using Volo.Abp.Modularity;

namespace Infocad.DynamicSpace;

[DependsOn(
    typeof(DynamicSpaceDomainModule),
    typeof(DynamicSpaceTestBaseModule)
)]
public class DynamicSpaceDomainTestModule : AbpModule
{

}
