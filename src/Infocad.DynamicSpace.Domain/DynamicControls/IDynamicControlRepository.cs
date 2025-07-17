using System;
using Volo.Abp.Domain.Repositories;

namespace Infocad.DynamicSpace.DynamicControls
{
    public interface IDynamicControlRepository : IRepository<DynamicControl, Guid>
    {
    }
}
