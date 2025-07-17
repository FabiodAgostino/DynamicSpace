using Volo.Abp.Modularity;

namespace Infocad.DynamicSpace;

[DependsOn(
    typeof(DynamicSpaceApplicationModule),
    typeof(DynamicSpaceDomainTestModule)
)]
public class DynamicSpaceApplicationTestModule : AbpModule
{

}
