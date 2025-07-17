using Infocad.DynamicSpace.DynamicTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Infocad.DynamicSpace.Totems
{
    public interface ITotemService : ICrudAppService<
        TotemDto, Guid, PagedAndSortedResultRequestDto,
        CreateTotemDto>
    {

    }
}
