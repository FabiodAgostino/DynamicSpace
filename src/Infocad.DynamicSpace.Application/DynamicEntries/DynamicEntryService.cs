using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infocad.DynamicSpace.DynamicEntries;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Infocad.DynamicSpace.DynamicEntry;

public class DynamicEntryService : DynamicSpaceAppService, IDynamicEntryService
{
    private readonly IDynamicEntryRepository _dynamicEntryRepository;

    public DynamicEntryService(IDynamicEntryRepository dynamicEntryRepository)
    {
        _dynamicEntryRepository = dynamicEntryRepository;
    }

    public async Task<PagedResultDto<DynamicEntryDto>> GetListEntryByEntityAsync(Guid idEntity,
        GetDynamicEntryListDto input)
    {
        var dynamicEntries = await _dynamicEntryRepository.GetListByEntityAsync(idEntity,
            input.SkipCount,
            input.MaxResultCount,
            input.Sorting,
            input.Filter
        );

        var totalCount = input.Filter == null
            ? await _dynamicEntryRepository.CountAsync()
            : await _dynamicEntryRepository.CountAsync(
                dynamicEntry => dynamicEntry.ExtraProperties.ContainsValue(input.Filter));

        return new PagedResultDto<DynamicEntryDto>(
            totalCount,
            ObjectMapper.Map<List<DynamicEntries.DynamicEntry>, List<DynamicEntryDto>>(dynamicEntries));
    }

    public async Task<DynamicEntryDto> CreateAsync(DynamicEntryDto input)
    {
        var dynmicEntry = ObjectMapper.Map<DynamicEntryDto, DynamicEntries.DynamicEntry>(input);
        var retval = await _dynamicEntryRepository.InsertAsync(dynmicEntry);

        return ObjectMapper.Map<DynamicEntries.DynamicEntry, DynamicEntryDto>(retval);
    }

    public async Task<DynamicEntryDto> UpdateAsync(Guid id, DynamicEntryDto input)
    {
        var updateEntry = ObjectMapper.Map<DynamicEntryDto, DynamicEntries.DynamicEntry>(input);
        try
        {
            var dynamicEntry = await _dynamicEntryRepository.GetAsync(id);
            await _dynamicEntryRepository.DeleteAsync(dynamicEntry);


            foreach (var value in input.ExtraProperties)
            {
                if (dynamicEntry.ExtraProperties.ContainsKey(value.Key))
                {
                    dynamicEntry.ExtraProperties[value.Key] = value.Value;
                }
                else
                {
                    dynamicEntry.ExtraProperties.Add(value.Key, value.Value);
                }
            }

            await _dynamicEntryRepository.UpdateAsync(dynamicEntry);

            return ObjectMapper.Map<DynamicEntries.DynamicEntry, DynamicEntryDto>(dynamicEntry);
        }
        catch (EntityNotFoundException ex)
        {
            throw new UserFriendlyException(L["DynamicEntryNotFound", id]);
        }
    }

    public async Task<DynamicEntryDto> DeleteAsync(Guid id)
    {
        try
        {
            var dynamicEntry = await _dynamicEntryRepository.GetAsync(id);
            await _dynamicEntryRepository.DeleteAsync(dynamicEntry);

            return ObjectMapper.Map<DynamicEntries.DynamicEntry, DynamicEntryDto>(dynamicEntry);
        }
        catch (EntityNotFoundException ex)
        {
            throw new UserFriendlyException(L["DynamicEntryNotFound", id]);
        }
    }

    public async Task<DynamicEntryDto> GetById(Guid id)
    {
        try
        {
            var dynamicEntry = await _dynamicEntryRepository.GetAsync(id);
            return ObjectMapper.Map<DynamicEntries.DynamicEntry, DynamicEntryDto>(dynamicEntry);
        }
        catch (EntityNotFoundException ex)
        {
            throw new UserFriendlyException(L["DynamicEntryNotFound", id]);
        }
    }

    public async Task<DynamicEntryDto> GetUpdateDynamicEntry(Guid id)
    {
        try
        {
            var dynamicEntry = await _dynamicEntryRepository.GetAsync(id);
            return ObjectMapper.Map<DynamicEntries.DynamicEntry, DynamicEntryDto>(dynamicEntry);
        }
        catch (EntityNotFoundException ex)
        {
            throw new UserFriendlyException(L["DynamicEntryNotFound", id]);
        }
    }
}