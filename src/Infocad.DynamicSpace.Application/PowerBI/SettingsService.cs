using Amazon.Runtime.Internal.Util;
using Infocad.DynamicSpace.Common;
using Infocad.GlobalConfig.Model;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Volo.Abp.SettingManagement;

namespace Infocad.DynamicSpace.PowerBI
{
    public class SettingsService : ISettingsService
    {
        //IMemoryCache _cache;
        //public SettingsService(HttpClient Http, IConfiguration Configuration, IMemoryCache cache) 
        //{
        //    _cache = cache;
        //}

        //public async Task<ServiceResponse<DescorAPIPowerBIConfig>> GetDescorPowerBIAPIConfig(string context, string key = "")
        //{
        //    if (_cache.TryGetValue(context + key, out DescorAPIPowerBIConfig descorAPIPowerBIConfig))
        //        return new ServiceResponse<DescorAPIPowerBIConfig> { Data = descorAPIPowerBIConfig, Success = true };
        //    else
        //    {
        //        string queryStringParam = $"context={context}&key={key}";
        //        var response = await RequestGet<DescorAPIPowerBIConfig>(SettingsType.GetDescorPowerBIAPIConfig, queryStringParam);
        //        _cache.Set(context + key, response);
        //        return response;
        //    }
        //}
    }
}
