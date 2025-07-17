using AutoMapper.Internal.Mappers;
using Infocad.DynamicSpace.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Infocad.DynamicSpace.DynamicEntities
{
    public class EFCoreDynamicEntityRepository : EfCoreRepository<DynamicSpaceDbContext, DynamicEntity, Guid>, IDynamicEntityRepository
    {
        public EFCoreDynamicEntityRepository(IDbContextProvider<DynamicSpaceDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<DynamicEntity> GetByIdAsync(Guid id)
        {
            return await base.GetAsync(id);
        }

        public async Task<List<DynamicEntity>> GetAllAsync()
        {
            return await base.GetListAsync();
        }

        public async Task<DynamicEntity> AddAsync(DynamicEntity entity)
        {
            return await base.InsertAsync(entity, autoSave: true);
        }

        public async Task<DynamicEntity> UpdateAsync(DynamicEntity entity)
        {
            return await base.UpdateAsync(entity, autoSave: true);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                await base.DeleteAsync(entity, autoSave: true);
            }
        }
        
        public async Task<List<DynamicEntity>> GetListByDynamicTypeAsync(
            Guid typeId)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet .Where(e => e.DynamicTypeId == typeId).ToListAsync();
        }
        
        public async Task<List<DynamicEntity>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string sorting,
            string filter = null)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet
                .WhereIf(
                    !filter.IsNullOrWhiteSpace(),
                    dynamic => dynamic.Name.Contains(filter)
                )
                .Include("Attributes")
                .OrderBy(sorting)
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync();
        }

        public async Task<DynamicEntity?> GetFullEntityByHybridEntity(string hybridTypeName)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.Include("Attributes").FirstOrDefaultAsync(x => x.HybridTypeName == hybridTypeName);
        }

        public async Task<List<DynamicEntity>> GetHybridEntitiesUsed()
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.Where(x => !String.IsNullOrEmpty(x.HybridTypeName)).ToListAsync();
        }

        public async Task<DynamicEntity> GetByIdIncludeAttributeAsync(Guid Id)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet
                .Include("Attributes")
                .FirstOrDefaultAsync(entity => Id != Guid.Empty && entity.Id == Id);
        }

        public async Task<DynamicEntity> FindByNameAsync(string name)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.FirstOrDefaultAsync(entity => entity.Name == name);
        }
    }
}
