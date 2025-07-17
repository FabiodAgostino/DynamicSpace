using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infocad.DynamicSpace.DynamicEntities;
using MongoDB.Driver;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;
namespace Infocad.DynamicSpace.MongoDB.DynammicEntities;

public class MongoDbDynamicEntityRepository : MongoDbRepository<DynamicSpaceMongoDbContext, DynamicEntity, Guid>, IDynamicEntityRepository
{

    public MongoDbDynamicEntityRepository(IMongoDbContextProvider<DynamicSpaceMongoDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<DynamicEntity> FindByNameAsync(string name)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.DynamicEntities
            .Find(e => e.Name == name)
            .FirstOrDefaultAsync();
    }

    public async Task<List<DynamicEntity>> GetListAsync(int skipCount, int maxResultCount, string sorting, string filter = null)
    {
        var dbContext = await GetDbContextAsync();
        return dbContext.DynamicEntities.AsQueryable()
            .WhereIf(
                !filter.IsNullOrWhiteSpace(),
                dynamic => dynamic.Name.Contains(filter)
            )
            //.Include("Attributes")
            .OrderBy(e => e.CreationTime)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToList();
    }

    public async Task<List<DynamicEntity>> GetListByDynamicTypeAsync(Guid typeId)
    {
        var dbContext = await GetDbContextAsync();
        return dbContext.DynamicEntities.AsQueryable().Where(e => e.DynamicTypeId == typeId).ToList();
    }

    public async Task<DynamicEntity> GetByIdIncludeAttributeAsync(Guid Id)
    {
        var dbContext = await GetDbContextAsync();
        return dbContext.DynamicEntities.AsQueryable()
            //.Include("Attributes")
            .FirstOrDefault(entity => Id != Guid.Empty && entity.Id == Id);
    }

    public async Task<DynamicEntity?> GetFullEntityByHybridEntity(string hybridTypeName)
    {
        var dbContext = await GetDbContextAsync();
        return dbContext.DynamicEntities.AsQueryable().FirstOrDefault(x => x.HybridTypeName == hybridTypeName);
    }

    public async Task<List<DynamicEntity>> GetHybridEntitiesUsed()
    {
        var dbContext = await GetDbContextAsync();
        return dbContext.DynamicEntities.AsQueryable().Where(x => !String.IsNullOrEmpty(x.HybridTypeName)).ToList();
    }
}