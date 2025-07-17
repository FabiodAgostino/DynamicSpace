using Infocad.DynamicSpace.DynamicAttributes;
using Infocad.DynamicSpace.DynamicTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Infocad.DynamicSpace.DynamicControls
{
    public interface IDynamicControlService : ICrudAppService<
            DynamicControlDto, Guid, PagedAndSortedResultRequestDto,
            CreateDynamicControlDto,UpdateDynamicControlDto>
    {
        Task<List<DynamicControlDto>> GetByNameAsync(string name);

        Task<List<DynamicControlDto>> GetByTypeAsync(DynamicAttributeType type);
    }
}
