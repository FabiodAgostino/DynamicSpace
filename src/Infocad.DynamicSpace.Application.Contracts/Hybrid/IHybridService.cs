using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Infocad.DynamicSpace.Hybrid
{
    public interface IHybridService<T,Y> : IApplicationService
    {
        Task<T> CreateAsync(T input);
        Task<T> DeleteAsync(Guid id);
        Task<PagedResultDto<T>> GetListObjects(Guid? idEntity, Y input);
        Task<T> GetById(Guid id);
        Task<T> GetUpdateObject(Guid id);
        Task<T> UpdateAsync(Guid id, T input);
        Task<List<T>> GetEntities();
    }
}
