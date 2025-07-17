using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Infocad.DynamicSpace.HybridRooms
{
    public class HybridRoomService : DynamicSpaceAppService, IHybridRoomService
    {
        private readonly IHybridRoomRepository _roomRepository;

        public HybridRoomService(IHybridRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public async Task<PagedResultDto<HybridRoomDto>> GetListObjects(Guid? idEntity,
            GetHybridRoomListDto input)
        {
            var rooms = await _roomRepository.GetListByEntityAsync(idEntity,
                input.SkipCount,
                input.MaxResultCount,
                input.Sorting,
                input.Filter
            );

            var totalCount = input.Filter == null
                ? await _roomRepository.CountAsync()
                : await _roomRepository.CountAsync(
                    room => room.ExtraProperties.ContainsValue(input.Filter) ||
                            room.Name.Contains(input.Filter) ||
                            room.Description.Contains(input.Filter));

            return new PagedResultDto<HybridRoomDto>(
                totalCount,
                ObjectMapper.Map<List<HybridRoom>, List<HybridRoomDto>>(rooms));
        }

        public async Task<HybridRoomDto> CreateAsync(HybridRoomDto input)
        {
            var room = ObjectMapper.Map<HybridRoomDto, HybridRoom>(input);
            var retval = await _roomRepository.InsertAsync(room);
            return ObjectMapper.Map<HybridRoom, HybridRoomDto>(retval);
        }

        public async Task<HybridRoomDto> UpdateAsync(Guid id, HybridRoomDto input)
        {
            try
            {
                var room = await _roomRepository.GetAsync(id);

                // Aggiornamento proprietà specifiche
                room.Name = input.Name;
                room.Capacity = input.Capacity;
                room.Description = input.Description;
                room.DynamicEntityId = input.DynamicEntityId;

                // Aggiornamento ExtraProperties
                foreach (var value in input.ExtraProperties)
                {
                    if (room.ExtraProperties.ContainsKey(value.Key))
                    {
                        room.ExtraProperties[value.Key] = value.Value;
                    }
                    else
                    {
                        room.ExtraProperties.Add(value.Key, value.Value);
                    }
                }

                await _roomRepository.UpdateAsync(room);
                return ObjectMapper.Map<HybridRoom, HybridRoomDto>(room);
            }
            catch (EntityNotFoundException ex)
            {
                throw new UserFriendlyException(L["RoomNotFound", id]);
            }
        }

        public async Task<List<HybridRoomDto>> GetEntities()
        {
            var rooms = await _roomRepository.GetListAsync();
            return ObjectMapper.Map<List<HybridRoom>, List<HybridRoomDto>>(rooms);
        }

        public async Task<HybridRoomDto> DeleteAsync(Guid id)
        {
            try
            {
                var room = await _roomRepository.GetAsync(id);
                await _roomRepository.DeleteAsync(room);
                return ObjectMapper.Map<HybridRoom, HybridRoomDto>(room);
            }
            catch (EntityNotFoundException ex)
            {
                throw new UserFriendlyException(L["RoomNotFound", id]);
            }
        }

        public async Task<HybridRoomDto> GetUpdateObject(Guid id)
        {
            try
            {
                var room = await _roomRepository.GetAsync(id);
                return ObjectMapper.Map<HybridRoom, HybridRoomDto>(room);
            }
            catch (EntityNotFoundException ex)
            {
                throw new UserFriendlyException(L["RoomNotFound", id]);
            }
        }

        public Task<PagedResultDto<HybridRoomDto>> GetListObjectsByEntityAsync<Y>(Guid idEntity, Y input)
        {
            throw new NotImplementedException();
        }

        public async Task<HybridRoomDto> GetById(Guid id)
        {
            try
            {
                var room = await _roomRepository.GetAsync(id);
                return ObjectMapper.Map<HybridRoom, HybridRoomDto>(room);
            }
            catch (EntityNotFoundException ex)
            {
                throw new UserFriendlyException(L["RoomNotFound", id]);
            }
        }
    }
}