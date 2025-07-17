using System;
using Infocad.DynamicSpace.DynamicRules;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace Infocad.DynamicSpace.MongoDB.DynamicRules;

public class MongoDbDynamicRuleRepository : MongoDbRepository<DynamicSpaceMongoDbContext, DynamicRule, Guid>,
    IDynamicRuleRepository
{
    public MongoDbDynamicRuleRepository(IMongoDbContextProvider<DynamicSpaceMongoDbContext> dbContextProvider) : base(
        dbContextProvider)
    {
    }
}