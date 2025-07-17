using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace Infocaad.DynamicSpace.MongoDB;

[ConnectionStringName(ConnectionStringName)]
public class DynamicSpaceMongoDbContext : AbpMongoDbContext, IDynamicSpaceMongoDbContext
{
    public const string ConnectionStringName = "DynamicSpace";
    
    /* Add mongo collections here. Example:
     * public IMongoCollection<Question> Questions => Collection<Question>();
     */

    protected override void CreateModel(IMongoModelBuilder modelBuilder)
    {
        base.CreateModel(modelBuilder);

        modelBuilder.ConfigureDynamicSpace();
    }
}
