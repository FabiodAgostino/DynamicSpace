using Volo.Abp.Modularity;

namespace Infocad.DynamicSpace;

public abstract class DynamicSpaceApplicationTestBase<TStartupModule> : DynamicSpaceTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
