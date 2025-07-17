using Infocad.DynamicSpace.EntityFrameworkCore;
using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Infocad.DynamicSpace.Totems
{
    public class EFCoreTotemRepository : EfCoreRepository<DynamicSpaceDbContext, Totem, Guid>, ITotemRepository
    {
        public EFCoreTotemRepository(IDbContextProvider<DynamicSpaceDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}
