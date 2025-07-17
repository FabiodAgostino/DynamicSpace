using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace Infocaad.DynamicSpace.MongoDB;

[ConnectionStringName(DynamicSpaceMongoDbContext.ConnectionStringName)]
public interface IDynamicSpaceMongoDbContext : IAbpMongoDbContext
{
    /* Define mongo collections here. Example:
     * IMongoCollection<Question> Questions { get; }
     */
}
