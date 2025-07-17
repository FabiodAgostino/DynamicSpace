using System;
using Infocad.DynamicSpace.DynamicTypes;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace Infocad.DynamicSpace.MongoDB.DynamicTypes;

public class MongoDbDynamicTypeRepository : MongoDbRepository<DynamicSpaceMongoDbContext, DynamicType, Guid>, IDynamicTypeRepository
{
    public MongoDbDynamicTypeRepository(IMongoDbContextProvider<DynamicSpaceMongoDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
}