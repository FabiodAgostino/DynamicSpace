using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Infocad.DynamicSpace.HybridBuildings
{
    public class HybridBuildingService : DynamicSpaceAppService, IHybridBuildingService
    {
        private readonly IHybridBuildingRepository _buildingRepository;

        public HybridBuildingService(IHybridBuildingRepository buildingRepository)
        {
            _buildingRepository = buildingRepository;
        }

        public async Task<PagedResultDto<HybridBuildingDto>> GetListObjects(Guid? idEntity,
            GetHybridBuildingListDto input)
        {
            var companies = await _buildingRepository.GetListByEntityAsync(idEntity,
                input.SkipCount,
                input.MaxResultCount,
                input.Sorting,
                input.Filter
            );

            var totalCount = input.Filter == null
                ? await _buildingRepository.CountAsync()
                : await _buildingRepository.CountAsync(
                    building => building.ExtraProperties.ContainsValue(input.Filter) ||
                               building.Name.Contains(input.Filter));

            return new PagedResultDto<HybridBuildingDto>(
                totalCount,
                ObjectMapper.Map<List<HybridBuilding>, List<HybridBuildingDto>>(companies));
        }

        public async Task<HybridBuildingDto> CreateAsync(HybridBuildingDto input)
        {
            var building = ObjectMapper.Map<HybridBuildingDto, HybridBuilding>(input);
            var retval = await _buildingRepository.InsertAsync(building);
            return ObjectMapper.Map<HybridBuilding, HybridBuildingDto>(retval);
        }

        public async Task<HybridBuildingDto> UpdateAsync(Guid id, HybridBuildingDto input)
        {
            try
            {
                var building = await _buildingRepository.GetAsync(id);

                // Aggiornamento proprietà specifiche
                building.X = input.X;
                building.Y = input.Y;
                building.Name = input.Name;
                building.DynamicEntityId = input.DynamicEntityId;

                // Aggiornamento ExtraProperties
                foreach (var value in input.ExtraProperties)
                {
                    if (building.ExtraProperties.ContainsKey(value.Key))
                    {
                        building.ExtraProperties[value.Key] = value.Value;
                    }
                    else
                    {
                        building.ExtraProperties.Add(value.Key, value.Value);
                    }
                }

                await _buildingRepository.UpdateAsync(building);
                return ObjectMapper.Map<HybridBuilding, HybridBuildingDto>(building);
            }
            catch (EntityNotFoundException ex)
            {
                throw new UserFriendlyException(L["BuildingNotFound", id]);
            }
        }

        public async Task<List<HybridBuildingDto>> GetEntities()
        {
            var buildings = await _buildingRepository.GetListAsync();
            return ObjectMapper.Map<List<HybridBuilding>, List<HybridBuildingDto>>(buildings);
        }

        public async Task<HybridBuildingDto> DeleteAsync(Guid id)
        {
            try
            {
                var building = await _buildingRepository.GetAsync(id);
                await _buildingRepository.DeleteAsync(building);
                return ObjectMapper.Map<HybridBuilding, HybridBuildingDto>(building);
            }
            catch (EntityNotFoundException ex)
            {
                throw new UserFriendlyException(L["BuildingNotFound", id]);
            }
        }

        public async Task<HybridBuildingDto> GetUpdateObject(Guid id)
        {
            try
            {
                var building = await _buildingRepository.GetAsync(id);
                return ObjectMapper.Map<HybridBuilding, HybridBuildingDto>(building);
            }
            catch (EntityNotFoundException ex)
            {
                throw new UserFriendlyException(L["BuildingNotFound", id]);
            }
        }

        public async Task<HybridBuildingDto> GetById(Guid id)
        {
            try
            {
                var building = await _buildingRepository.GetAsync(id);
                return ObjectMapper.Map<HybridBuilding, HybridBuildingDto>(building);
            }
            catch (EntityNotFoundException ex)
            {
                throw new UserFriendlyException(L["BuildingNotFound", id]);
            }
        }
    }
}
