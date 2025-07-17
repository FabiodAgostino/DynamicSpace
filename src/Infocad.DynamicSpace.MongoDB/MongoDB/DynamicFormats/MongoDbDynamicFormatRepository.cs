using System;
using Infocad.DynamicSpace.DynamicFormats;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace Infocad.DynamicSpace.MongoDB.DynamicFormats;

public class MongoDbDynamicFormatRepository : MongoDbRepository<DynamicSpaceMongoDbContext, DynamicFormat, Guid>,
    IDynamicFormatRepository
{
    public MongoDbDynamicFormatRepository(IMongoDbContextProvider<DynamicSpaceMongoDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}