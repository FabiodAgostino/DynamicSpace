using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Infocad.DynamicSpace.DynamicAttirbutes;

public interface IDynamicAttributeRepository : IRepository<DynamicAttribute,Guid>
{
    Task<List<DynamicAttribute>> GetListByGuidsAsync(List<Guid> Ids);
}