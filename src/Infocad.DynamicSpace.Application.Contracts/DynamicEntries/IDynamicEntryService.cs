using System;
using System.Threading.Tasks;
using Infocad.DynamicSpace.DynamicEntries;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Infocad.DynamicSpace.DynamicEntry;

public interface IDynamicEntryService : IApplicationService
{
    Task<PagedResultDto<DynamicEntryDto>> GetListEntryByEntityAsync(Guid idEntity, GetDynamicEntryListDto input);
    Task<DynamicEntryDto> CreateAsync(DynamicEntryDto input);
    Task<DynamicEntryDto> UpdateAsync(Guid id, DynamicEntryDto input);
    Task<DynamicEntryDto> DeleteAsync(Guid id);
    Task<DynamicEntryDto> GetUpdateDynamicEntry(Guid id);
    Task<DynamicEntryDto> GetById(Guid id);
}