using Infocad.DynamicSpace.Common;
using Infocad.GlobalConfig.Model;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Infocad.DynamicSpace.PowerBI
{
    public interface ISettingsService : IApplicationService
    {
        //Task<ServiceResponse<DescorAPIPowerBIConfig>> GetDescorPowerBIAPIConfig(string context, string key = "");
    }
}
