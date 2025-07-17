using System;
using Infocad.DynamicSpace.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Infocad.DynamicSpace.DynamicTypes;

public class EFCoreDynamicTypeRepository : EfCoreRepository<DynamicSpaceDbContext,DynamicType,Guid> , IDynamicTypeRepository
{
    public EFCoreDynamicTypeRepository(IDbContextProvider<DynamicSpaceDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
}