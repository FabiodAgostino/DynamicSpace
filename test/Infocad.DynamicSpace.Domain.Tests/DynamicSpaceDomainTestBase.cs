using Volo.Abp.Modularity;

namespace Infocad.DynamicSpace;

/* Inherit from this class for your domain layer tests. */
public abstract class DynamicSpaceDomainTestBase<TStartupModule> : DynamicSpaceTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
