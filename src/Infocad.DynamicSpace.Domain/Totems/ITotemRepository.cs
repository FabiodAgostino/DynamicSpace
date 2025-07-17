using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Infocad.DynamicSpace.Totems
{
    public interface ITotemRepository : IRepository<Totem, Guid>
    {

    }
}
