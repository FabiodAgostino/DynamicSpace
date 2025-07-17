using Autofac.Core;
using Infocad.DynamicSpace.Common;
using Infocad.GlobalConfig.Model;
using InfocadPowerBIModels.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Infocad.DynamicSpace.Blazor.Client.Pages
{
    public partial class PowerBI
    {

        private EmbedParams PowerBIObject { get; set; }
        public bool IsLoading {get;set; } = false;
        public string? Error { get; set; } = null;
        private string ExtraBaseUrl = "api";

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                HttpClient = HttpClientFactory.CreateClient("InfocadServerAPI");
                var powerBIObject = await LoadPowerBI();
                await Interop.EmbedItemPowerBI(JSRuntime, PowerBIElement, powerBIObject, ParsePowerBIFilters(SubParams));
            }
        }


        public async Task<EmbedParams> LoadPowerBI()
        {
            IsLoading = true;
            if (String.IsNullOrEmpty(ItemNameParam))
            {
                Error = "Item name not specified.";
                return PowerBIObject;
            }

            if (String.IsNullOrEmpty(WorkspaceNameParam))
            {
                Error = "Workspace name not specified.";
                return PowerBIObject;
            }

            if (!String.IsNullOrEmpty(ItemTypeParam) && ItemTypeParam.ToLower() == "d")
            {
                var dashboardFilter = new DashboardFilter() { DashboardName = ItemNameParam, WorkSpaceName = WorkspaceNameParam, UseRLS = UseRLSParam };
                if (UseRLSParam)
                {
                    dashboardFilter.UserName = "admin";
                    dashboardFilter.Roles = new();
                }
                return await GetDashboardEmbedInfoByFilters(dashboardFilter);
            }
            else
            {
                var reportFilter = new ReportFilter() { ReportName = ItemNameParam, WorkSpaceName = WorkspaceNameParam, UseRLS = UseRLSParam };
                if (UseRLSParam)
                {
                    reportFilter.UserName = "admin";
                    reportFilter.Roles = new();
                }
                return await GetReportEmbedInfoByFilters(reportFilter);
            }
            IsLoading = false;
        }
        /// <summary>
        /// Retrieves embedding parameters for a Power BI dashboard.
        /// </summary>
        /// <param name="filter">Dashboard filter parameters.</param>
        /// <returns>Embedded Power BI parameters.</returns>
        private async Task<EmbedParams> GetDashboardEmbedInfoByFilters(DashboardFilter filter)
        {
            try
            {
                await SetHttpClientBaseUrl();
                var result = await RequestPost<DashboardFilter, EmbedParams>("v1/PowerBI/GetDashboardEmbedInfoByFilters", filter);
                if (result != null)
                    PowerBIObject = result;
                else
                    Error = "Something went wrong.";

                return PowerBIObject;

            }
            catch (Exception ex)
            {
                Error = ex.Message;
                return PowerBIObject;
            }
            finally
            {
                StateHasChanged();
            }
        }

        /// <summary>
        /// Retrieves embedding parameters for a Power BI report.
        /// </summary>
        /// <param name="filter">Report filter parameters.</param>
        /// <returns>Embedded Power BI parameters.</returns>
        private async Task<EmbedParams> GetReportEmbedInfoByFilters(ReportFilter filter)
        {
            try
            {
                await SetHttpClientBaseUrl();
                var result = await RequestPost<ReportFilter, EmbedParams>("v1/PowerBI/GetReportEmbedInfoByFilters", filter);
                if (result != null)
                    PowerBIObject = result;
                else
                    Error = "Something went wrong.";

                return PowerBIObject;
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                return PowerBIObject;
            }
            finally
            {
                StateHasChanged();
            }
        }

        private async Task SetHttpClientBaseUrl()
        {
            ServiceResponse<DescorAPIPowerBIConfig> response = await GetDescorPowerBIAPIConfig("InfocadPowerBI", "DescorConfig");
            if (response.Success)
            {
                HttpClient = new HttpClient();
                HttpClient.BaseAddress = new Uri(response.Data.WebApiBaseUrl);
            }
            else
                throw new Exception("Descor PowerBI API configuration not found.");
        }

        private async Task<ServiceResponse<DescorAPIPowerBIConfig>> GetDescorPowerBIAPIConfig(string context, string key = "")
        {
            if (MemoryCache.TryGetValue(context + key, out DescorAPIPowerBIConfig descorAPIPowerBIConfig))
                return new ServiceResponse<DescorAPIPowerBIConfig> { Data = descorAPIPowerBIConfig, Success = true };
            else
            {
                string queryStringParam = $"context={context}&key={key}";
                var response = await RequestGet<DescorAPIPowerBIConfig>("Settings/GetDescorPowerBIAPIConfig", queryStringParam);
                MemoryCache.Set(context + key, response);
                return response;
            }
        }

        public async virtual Task<ServiceResponse<Q>> RequestGet<Q>(string endpoint, string queryString)
        {
            try
            {
                string queryS;
                if (!string.IsNullOrEmpty(queryString))
                {
                    queryS = string.Format("{0}?{1}", GetCommandUrl(endpoint), queryString);
                }
                else
                {
                    queryS = GetCommandUrl(endpoint);
                }
                HttpResponseMessage res = await HttpClient.GetAsync(queryS);
                if (res.IsSuccessStatusCode || res.StatusCode == HttpStatusCode.BadRequest)
                {
                    var result = await res.Content.ReadFromJsonAsync<ServiceResponse<Q>>();
                    return result;
                }

                var message = res.Content.ReadAsStringAsync();
                var ex = new Exception($"The service returned with status {res.StatusCode} message:{message}");
                throw ex;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }

        }


        public async Task<R> RequestPost<Q, R>(string queryString, Q body)
        {
            HttpResponseMessage res = await HttpClient.PostAsJsonAsync<Q>(GetCommandUrl(queryString), body);

            if (res.IsSuccessStatusCode || res.StatusCode == HttpStatusCode.BadRequest)
                return await res.Content.ReadFromJsonAsync<R>();
            else
            {
                string error = await res.Content.ReadAsStringAsync();
                throw new Exception($"The service returned with status {res.StatusCode} message:{error}");
            }
        }

        private string GetCommandUrl(string query)
        {
            return string.Format("{0}/{1}", ExtraBaseUrl, query);
        }

        /// <summary>
        /// Parses the filter parameters from the query string for Power BI and returns them as a JSON string.
        /// If the filter parameters are not present or there is an error parsing, an empty array is returned.
        /// </summary>
        /// <returns>A JSON string representing the decoded Power BI filter parameters, or an empty array if there is an error.</returns>
        public List<string> ParsePowerBIFilters(string subparams)
        {
            var listparams = new List<string>();
            if (String.IsNullOrEmpty(subparams))
                return new List<string>();

            try
            {
                // Try to parse the JSON string
                var json = JArray.Parse(subparams);

                // List of expected keys
                var requiredKeys = new HashSet<string> { "table", "column", "value" };

                foreach (var item in json)
                {
                    if (item is JObject obj)
                    {
                        var keys = obj.Properties().Select(p => p.Name.ToLower()).ToHashSet();

                        // Check if any required keys are missing
                        if (!requiredKeys.IsSubsetOf(keys))
                        {
                            Error = $"Invalid subparams: missing or incorrect properties. Expected keys: {string.Join(", ", requiredKeys)}";
                            return new List<string>();
                        }
                        listparams.Add(obj.ToString());
                    }
                    else
                    {
                        Error = "Invalid subparams format.";
                        return new List<string>();
                    }
                }
                return listparams;
            }
            catch (Exception ex)
            {
                Error = "Error parsing subparams string.";
                return new List<string>();
            }
        }
    }
}
