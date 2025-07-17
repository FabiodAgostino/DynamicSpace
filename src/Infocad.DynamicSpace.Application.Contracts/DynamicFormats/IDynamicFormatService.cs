using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Infocad.DynamicSpace.DynamicFormats
{
    public interface IDynamicFormatService : ICrudAppService<
    DynamicFormatDto, Guid, PagedAndSortedResultRequestDto,
    CreateDynamicFormatDto, UpdateDynamicFormatDto>
    {
    }
}
