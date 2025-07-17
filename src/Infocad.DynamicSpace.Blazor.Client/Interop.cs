using InfocadPowerBIModels.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infocad.DynamicSpace.Blazor.Client
{
    public static class Interop
    {
        internal static ValueTask<object> EmbedItemPowerBI(
            IJSRuntime jsRuntime,
            ElementReference reportContainer,
            EmbedParams embedParams,
            List<string> subParameters = null
            )
        {
            return jsRuntime.InvokeAsync<object>("embedItemPowerBI", embedParams, reportContainer, subParameters);
        }
    }
}
