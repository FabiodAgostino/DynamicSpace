using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infocad.DynamicSpace.DynamicAttirbutes;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace Infocad.DynamicSpace.MongoDB.DynamicAttributes;

public class MongoDbDynamicAttributeRepository : MongoDbRepository<DynamicSpaceMongoDbContext, DynamicAttribute, Guid>
    , IDynamicAttributeRepository
{
    public MongoDbDynamicAttributeRepository(IMongoDbContextProvider<DynamicSpaceMongoDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<List<DynamicAttribute>> GetListByGuidsAsync(List<Guid> Ids)
    {
        var queryable = await GetMongoQueryableAsync();
        return await queryable
            .Where(attribute => Ids.Contains(attribute.Id))
            .ToListAsync();
    }
}