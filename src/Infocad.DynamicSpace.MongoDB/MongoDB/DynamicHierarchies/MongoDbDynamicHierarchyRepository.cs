using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infocad.DynamicSpace.DynamicHierarchies;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace Infocad.DynamicSpace.MongoDB.DynamicHierarchies;

public class MongoDbDynamicHierarchyRepository :  MongoDbRepository<DynamicSpaceMongoDbContext, DynamicHierarchy, Guid>,
    IDynamicHierarchyRepository
{
    public MongoDbDynamicHierarchyRepository(IMongoDbContextProvider<DynamicSpaceMongoDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<DynamicHierarchy> FindByNameAsync(string name)
    {
        return (await GetMongoQueryableAsync()).FirstOrDefault(h => h.Name == name);
    }

    public Task<List<DynamicHierarchy>> GetListAsync(int skipCount, int maxResultCount, string sorting, string filter = null)
    {
        throw new NotImplementedException();
    }

    public Task<DynamicHierarchy> GetByIdIncludeEntitiesAsync(Guid Id)
    {
        throw new NotImplementedException();
    }
}