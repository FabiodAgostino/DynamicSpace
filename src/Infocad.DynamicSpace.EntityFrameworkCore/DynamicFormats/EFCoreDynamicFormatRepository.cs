using System;
using Infocad.DynamicSpace.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Infocad.DynamicSpace.DynamicFormats;

public class EFCoreDynamicFormatRepository : EfCoreRepository<DynamicSpaceDbContext, DynamicFormat, Guid>,
    IDynamicFormatRepository
{
    public EFCoreDynamicFormatRepository(IDbContextProvider<DynamicSpaceDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
}