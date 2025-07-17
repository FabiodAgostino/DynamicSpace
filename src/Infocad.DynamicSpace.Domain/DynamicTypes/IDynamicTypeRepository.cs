using System;
using Volo.Abp.Domain.Repositories;

namespace Infocad.DynamicSpace.DynamicTypes;

public interface IDynamicTypeRepository : IRepository<DynamicType, Guid>
{
    
}