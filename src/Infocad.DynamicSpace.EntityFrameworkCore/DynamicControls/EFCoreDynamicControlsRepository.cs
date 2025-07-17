using Infocad.DynamicSpace.EntityFrameworkCore;
using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Infocad.DynamicSpace.DynamicControls
{
    public class EFCoreDynamicControlRepository : EfCoreRepository<DynamicSpaceDbContext, DynamicControl, Guid>, IDynamicControlRepository
    {
        public EFCoreDynamicControlRepository(IDbContextProvider<DynamicSpaceDbContext> dbContextProvider) : base(
       dbContextProvider)
        {

        }
    }
}