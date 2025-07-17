using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infocad.DynamicSpace.DynamicEntityAttributes;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;


using Volo.Abp.Uow;
using AutoMapper.Internal.Mappers;
using Infocad.DynamicSpace.DynamicAttributes;
using Infocad.DynamicSpace.DynamicEntities;
using Infocad.DynamicSpace.HybridBuildings;
using Microsoft.EntityFrameworkCore;


namespace Infocad.DynamicSpace.DynamicEntities
{
    public class DynamicEntityService : DynamicSpaceAppService, IDynamicEntityService
    {
        private readonly IDynamicEntityRepository _dynamicEntityRepository;
        private readonly DynamicEntityManager _dynamicEntityManager;
    
        public DynamicEntityService(IDynamicEntityRepository dynamicEntityRepository, DynamicEntityManager dynamicEntityManager)
        {
            _dynamicEntityRepository = dynamicEntityRepository;
            _dynamicEntityManager = dynamicEntityManager;
        }
        
        public async Task<List<DynamicEntityDto>> GetListByDynamicTypeAsync(Guid id)
        {
            var entity = await _dynamicEntityRepository.GetListByDynamicTypeAsync(id);
            return ObjectMapper.Map<List<DynamicEntity>, List<DynamicEntityDto>>(entity);
        }

        public async Task<DynamicEntityDto?> GetFullEntityByHybridEntity(string hybridTypeName)
        {
            var entity = await _dynamicEntityRepository.GetFullEntityByHybridEntity(hybridTypeName);
            if(entity != null)
                return ObjectMapper.Map<DynamicEntity, DynamicEntityDto>(entity);
            else
                return null;
        }

        public async Task<List<DynamicEntityDto>> GetHybridEntitiesUsed()
        {
            var entity = await _dynamicEntityRepository.GetHybridEntitiesUsed();
            if (entity != null)
                return ObjectMapper.Map<List<DynamicEntity>, List<DynamicEntityDto>>(entity);
            else
                return new();
        }

        public Task<List<DynamicEntityDto>> GetHybridEntities()
        {
            var domainAssembly = typeof(HybridBuildingDto).Assembly;
            var hybridTypes = domainAssembly.GetTypes()
                .Where(t => t.Name.StartsWith("Hybrid") &&
                            t.IsClass &&
                            !t.IsAbstract)
                .ToList();
            var retval = hybridTypes.Select(t => new DynamicEntityDto
            {
                Id = Guid.Empty, // Placeholder, as we don't have an ID for these types
                Name = t.Name,
                Description = t.FullName,
                DynamicTypeId = Guid.Empty, // Placeholder, as we don't have a DynamicTypeId for these types
                IsHybrid = true,
                HybridTypeName = t.AssemblyQualifiedName
            }).ToList();
            return retval.Any() ? Task.FromResult(retval) : Task.FromResult(new List<DynamicEntityDto> { null });
        }


        public async Task<PagedResultDto<DynamicEntityDto>> GetListAsync(GetDynamicEntityListDto input)
        {
            if (input.Sorting.IsNullOrWhiteSpace())
            {
                input.Sorting = nameof(DynamicEntityDto.Name);
            }

            var dynamicEntities = await _dynamicEntityRepository.GetListAsync(
                input.SkipCount,
                input.MaxResultCount,
                input.Sorting,
                input.Filter
            );

            var totalCount = input.Filter == null
                ? await _dynamicEntityRepository.CountAsync()
                : await _dynamicEntityRepository.CountAsync(
                    dynamicEntity => dynamicEntity.Name.Contains(input.Filter));

            return new PagedResultDto<DynamicEntityDto>(
                totalCount,
                ObjectMapper.Map<List<DynamicEntity>, List<DynamicEntityDto>>(dynamicEntities)
            );
        }

        public async Task<List<DynamicEntityDto>> GetAllDynamicEntity()
        {
            var entities = await _dynamicEntityRepository.GetListAsync();
            return ObjectMapper.Map<List<DynamicEntity>, List<DynamicEntityDto>>(entities);
        }

        public async Task<DynamicEntityDto> GetByIdIncludeAttributeAsync(Guid id)
        {
            var entity = await _dynamicEntityRepository.GetByIdIncludeAttributeAsync(id);
            return ObjectMapper.Map<DynamicEntity, DynamicEntityDto>(entity);
        }

        public async Task<DynamicEntityDto> UpdateAttribute(UpdateDynamicEntityAttributeDto input)
        {
            var entity = await _dynamicEntityRepository.GetAsync(input.DynamicEntityId);
            var attribute = entity.Attributes.FirstOrDefault(x => x.Id == input.Id);
            if (attribute == null)
            {
                throw new UserFriendlyException("Attribute not found");
            }
    
            attribute.ChangeLabel(input.Label);
            attribute.ChangeOrder(input.Order);
    
            await _dynamicEntityRepository.UpdateAsync(entity);
    
            return ObjectMapper.Map<DynamicEntity, DynamicEntityDto>(entity);
        }
    
    
        public async Task<DynamicEntityDto> CreateAsync(CreateDynamicEntityDto input)
        {

            var entity = await _dynamicEntityManager.CreateAsync(input.Name,
                input.Description,
                input.DynamicTypeId, input.IsHybrid, input.HybridTypeName);
            
            if (input.Attributes != null && input.Attributes.Any())
            {
   
                foreach (var attrDto in input.Attributes)
                {
                    
                    await _dynamicEntityManager.AddAttributeAsync(entity, attrDto.DynamicAttributeId,
                        attrDto.Order, attrDto.Label, attrDto.DynamicFormatId, attrDto.DynamicRuleId, attrDto.DynamicControlId, required:attrDto.Required);
                }
            }
            var savedEntity = await _dynamicEntityRepository.InsertAsync(entity);
            return ObjectMapper.Map<DynamicEntity, DynamicEntityDto>(savedEntity);
    
        }
        
        public async Task<DynamicEntityDto> CreateAttributeAsync(CreateDynamicEntityAttributeDto input)
        {
            var entity = await _dynamicEntityRepository.GetAsync(input.DynamicEntityId);
            
            await _dynamicEntityManager.AddAttributeAsync(entity, input.DynamicAttributeId,input.Order,input.Label, input.DynamicFormatId, input.DynamicRuleId, input.DynamicControlId);
    
            await _dynamicEntityRepository.UpdateAsync(entity);
    
            return ObjectMapper.Map<DynamicEntity, DynamicEntityDto>(entity);
        }

        public async Task<DynamicEntityDto> DeleteAttributeAsync(Guid idEntity, Guid idAttribute)
        {
            var entity = await _dynamicEntityRepository.GetByIdIncludeAttributeAsync(idEntity);
            
            var attribute = entity.Attributes.FirstOrDefault(x => x.DynamicAttributeId == idAttribute);
            if (attribute == null)
            {
                throw new UserFriendlyException("Attribute not found");
            }
            entity.RemoveAttribute(attribute);
            return ObjectMapper.Map<DynamicEntity, DynamicEntityDto>(entity);
        }

        public async Task UpdateAsync(UpdateDynamicEntityDto input)
        {
            var entity = await _dynamicEntityRepository.GetByIdIncludeAttributeAsync(input.Id);

            // Converti tutti gli attributi di input in DynamicEntityAttribute
            var allNewAttributes = input.Attributes
                .Select(attr => ObjectMapper.Map<CreateDynamicEntityAttributeDto, DynamicEntityAttribute>(attr))
                .ToList();

            if (entity.CheckEdit(input.Name, input.Description, input.DynamicTypeId, input.HybridTypeName) || input.Attributes.Any())
            {
                try
                {
                    await _dynamicEntityManager.ChangeNameAsync(entity, input.Name);
                    entity.Description = input.Description;
                    entity.HybridTypeName = input.HybridTypeName;
                    if (!String.IsNullOrEmpty(input.HybridTypeName))
                        entity.IsHybrid = true;
                    if (input.Attributes != null && input.Attributes.Any())
                    {
                        await _dynamicEntityManager.ReplaceAllAttributesAsync(entity, allNewAttributes);
                    }

                    await _dynamicEntityRepository.UpdateAsync(entity);
                }
                catch (BusinessException e)
                {
                    throw new UserFriendlyException(e.Message);
                }
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            await _dynamicEntityRepository.DeleteAsync(id);
        }
        
        public async Task<List<DynamicEntityDto>> GetNoHybridEntities()
        {
            var entity = await _dynamicEntityRepository.GetListAsync(d => !d.IsHybrid);
            return ObjectMapper.Map<List<DynamicEntity>, List<DynamicEntityDto>>(entity);
        }

       
    }
}






