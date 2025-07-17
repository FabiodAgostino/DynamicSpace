using AutoMapper.Internal.Mappers;
using Infocad.DynamicSpace.DynamicAttributes;
using Infocad.DynamicSpace.DynamicTypes;
using Infocad.DynamicSpace.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Infocad.DynamicSpace.DynamicControls
{
    public class DynamicControlService : CrudAppService<
        DynamicControl,
        DynamicControlDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateDynamicControlDto, UpdateDynamicControlDto>, IDynamicControlService
    {
        private readonly IRepository<DynamicControl, Guid> _dynamicControlRepository;
        public DynamicControlService(IDynamicControlRepository repository) : base(repository)
        {
            GetPolicyName = DynamicSpacePermissions.DynamicType.Default;
            GetListPolicyName = DynamicSpacePermissions.DynamicType.Default;
            CreatePolicyName = DynamicSpacePermissions.DynamicType.Create;
            UpdatePolicyName = DynamicSpacePermissions.DynamicType.Edit;
            DeletePolicyName = DynamicSpacePermissions.DynamicType.Delete;
        }

        public async Task<List<DynamicControlDto>> GetByNameAsync(string name)
        {
            var queryable = await _dynamicControlRepository.GetQueryableAsync();

            var dynamicControls = await AsyncExecuter.ToListAsync(
                queryable.Where(dc => dc.Name.Contains(name))
            );

            return ObjectMapper.Map<List<DynamicControl>, List<DynamicControlDto>>(dynamicControls);
        }

        public async Task<List<DynamicControlDto>> GetByTypeAsync(DynamicAttributeType type)
        {
            var queryable = await _dynamicControlRepository.GetQueryableAsync();

            var dynamicControls = await AsyncExecuter.ToListAsync(
                queryable.Where(dc => dc.Type == type)
            );

            return ObjectMapper.Map<List<DynamicControl>, List<DynamicControlDto>>(dynamicControls);
        }
    }
}
