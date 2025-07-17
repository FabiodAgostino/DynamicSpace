using Volo.Abp;
using Volo.Abp.MongoDB;

namespace Infocaad.DynamicSpace.MongoDB;

public static class DynamicSpaceMongoDbContextExtensions
{
    public static void ConfigureDynamicSpace(
        this IMongoModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));
    }
}
