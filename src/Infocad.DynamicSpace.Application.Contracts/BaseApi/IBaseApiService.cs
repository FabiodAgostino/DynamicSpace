using Infocad.DynamicSpace.Common;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Infocad.DynamicSpace.BaseApi
{
    public interface IBaseApiService<T> : IApplicationService where T : class
    {
        Task<ServiceResponse<Q>> PostItem<Q>(T item, string endpoint);
        Task<ServiceResponse<Q?>> RequestGet<Q>(string endpoint, string queryString);
    }
}
