using InfocadPowerBIModels.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Infocad.DynamicSpace.PowerBI
{
    public class PowerBIService : IPowerBIService
    {
    //    public PowerBIService(HttpClient Http, IConfiguration Configuration, ISettingsService settingsService, ProtectedSessionStorage storage) 
    //    {
    //        ExtraBaseUrl = "api";
    //        _settingsService = settingsService;
    //        _storage = storage;
    //    }
    //    /// <summary>
    //    /// Retrieves embed parameters for a Power BI report based on provided filters.
    //    /// </summary>
    //    /// <param name="filter">Report filter parameters.</param>
    //    /// <returns>Embed parameters for the specified report.</returns>
    //    public async Task<EmbedParams> GetReportEmbedInfoByFilters(ReportFilter filter)
    //    {
    //        await SetHttpClientBaseUrl();
    //        return await RequestPost<ReportFilter, EmbedParams>("v1/PowerBI/GetReportEmbedInfoByFilters", filter);
    //    }

    //    /// <summary>
    //    /// Retrieves embed parameters for a Power BI dashboard based on provided filters.
    //    /// </summary>
    //    /// <param name="filter">Dashboard filter parameters.</param>
    //    /// <returns>Embed parameters for the specified dashboard.</returns>
    //    public async Task<EmbedParams> GetDashboardEmbedInfoByFilters(DashboardFilter filter)
    //    {
    //        await SetHttpClientBaseUrl();
    //        return await RequestPost<DashboardFilter, EmbedParams>("v1/PowerBI/GetDashboardEmbedInfoByFilters", filter);
    //    }

    //    private async Task SetHttpClientBaseUrl()
    //    {
    //        ServiceResponse<DescorAPIPowerBIConfig> response = await _settingsService.GetDescorPowerBIAPIConfig("InfocadPowerBI", "DescorConfig");
    //        if (response.Success)
    //        {
    //            HttpClient.BaseAddress = new Uri(response.Data.WebApiBaseUrl);
    //            var savedToken = (await _storage.GetAsync<string>("token-auth")).Value;
    //            if (String.IsNullOrEmpty(savedToken))
    //                throw new Exception("Token not found.");

    //            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);
    //        }
    //        else
    //            throw new Exception("Descor PowerBI API configuration not found.");
    //    }


    }
}
