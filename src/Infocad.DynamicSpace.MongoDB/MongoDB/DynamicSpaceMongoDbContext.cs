using Infocad.DynamicSpace.DynamicAttirbutes;
using Infocad.DynamicSpace.DynamicEntities;
using Infocad.DynamicSpace.DynamicEntries;
using Infocad.DynamicSpace.DynamicFormats;
using Infocad.DynamicSpace.DynamicRules;
using Infocad.DynamicSpace.DynamicTypes;
using Infocad.DynamicSpace.HybridBuildings;
using Infocad.DynamicSpace.HybridCompanies;
using Infocad.DynamicSpace.HybridRooms;
using MongoDB.Driver;
using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace Infocad.DynamicSpace.MongoDB;

[ConnectionStringName(ConnectionStringName)]
public class DynamicSpaceMongoDbContext : AbpMongoDbContext
{
    public const string ConnectionStringName = "MongoDefault";


    public IMongoCollection<DynamicType> DynamicTypes => Collection<DynamicType>();
    public IMongoCollection<DynamicAttribute> DynamicAttributes => Collection<DynamicAttribute>();
    public IMongoCollection<DynamicEntity> DynamicEntities => Collection<DynamicEntity>();
    public IMongoCollection<DynamicEntityAttribute> DynamicEntityAttributes => Collection<DynamicEntityAttribute>();
    public IMongoCollection<DynamicEntry> DynamicEntries => Collection<DynamicEntry>();
    public IMongoCollection<DynamicFormat> DynamicFormats => Collection<DynamicFormat>();
    public IMongoCollection<DynamicRule> DynamicRules => Collection<DynamicRule>();

    public IMongoCollection<HybridBuilding> HybridBuildings => Collection<HybridBuilding>();
    public IMongoCollection<HybridCompany> HybridCompanies => Collection<HybridCompany>();
    public IMongoCollection<HybridRoom> HybridRooms => Collection<HybridRoom>();




    protected override void CreateModel(IMongoModelBuilder modelBuilder)
    {
        base.CreateModel(modelBuilder);

        //modelBuilder.ConfigureDynamicSpace();
    }
}