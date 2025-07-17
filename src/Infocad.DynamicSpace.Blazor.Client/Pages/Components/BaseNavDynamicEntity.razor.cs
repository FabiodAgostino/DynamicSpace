using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Infocad.DynamicSpace.DynamicAttributes;
using Infocad.DynamicSpace.DynamicEntries;
using Infocad.DynamicSpace.DynamicEntry;
using Infocad.DynamicSpace.Hybrid;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Primitives;
namespace Infocad.DynamicSpace.Blazor.Client.Pages.Components;
partial class BaseNavDynamicEntity
{
    public string SelectedValue { get; set; }

    [Parameter] public NavSettingDto NavAttribute { get; set; }
    private string _oldEntity = string.Empty;
    public IDictionary<Guid, string> Values { get; set; } = new Dictionary<Guid, string>();
    public string Domain { get; set; } = string.Empty;
    protected override async Task OnParametersSetAsync()
    {
        if(String.IsNullOrEmpty(_oldEntity) || _oldEntity != NavAttribute.EntityName)
        {
            Values.Clear();
            _oldEntity = NavAttribute.EntityName;
            await Refresh();
        }
    }

    private async Task Refresh()
    {
        if (NavAttribute != null && NavAttribute.IsHybrid)
        {

            var fullname = NavAttribute.FullQualifieldName.Split(',')[0];
            Domain = NavAttribute.FullQualifieldName;
            var typeName = fullname.Split(".")[3];

            var getTypeName = typeName
                .Replace("Dto", "Service")
                .Replace("Hybrid", "IHybrid");

            var type = Type.GetType(NavAttribute.FullQualifieldName.Replace(typeName, getTypeName));
            object? service = ServiceProvider.GetService(type);

            var method = service.GetType().GetMethod("GetEntities");
            var task = method.Invoke(service, null);

            await (Task)task;

            var resultProperty = task.GetType().GetProperty("Result");
            if (resultProperty != null)
            {
                object? entities = resultProperty.GetValue(task);
                var entitiesList = (IEnumerable<object>)entities;
                PopulateComboHybrid(entitiesList);
            }
        }
        else
        {
            var results = await DynamicEntryService.GetListEntryByEntityAsync(NavAttribute.Entity, new GetDynamicEntryListDto() { MaxResultCount = 1000 });
            Domain = typeof(DynamicEntryDto).AssemblyQualifiedName ?? string.Empty;
            PopulateComboDynamic(results.Items);
        }
    }

    private void PopulateComboHybrid(IEnumerable<object> entitiesList)
    {
        foreach (var ent in entitiesList)
        {
            string valueToShow = string.Empty;
            Guid id = new Guid(ent.GetType().GetProperty("Id").GetValue(ent).ToString());
            try
            {
                valueToShow = ent.GetType().GetProperty("Name")?.GetValue(ent)?.ToString() ?? string.Empty;
                //se non esistrno properietà nome 
                if (String.IsNullOrEmpty(valueToShow))
                {
                    var stringProperties = ent.GetType().GetProperties()
                       .Where(p => p.PropertyType == typeof(string) && p.CanRead)
                       .ToList();
                    if(stringProperties.Any())
                    {
                        string fallbackValue = string.Empty;
                        foreach (var prop in stringProperties)
                        {
                            var value = prop.GetValue(ent)?.ToString();
                            if (!string.IsNullOrWhiteSpace(value))
                            {
                                valueToShow = value;
                                break;
                            }
                        }
                    }
                    //se non esistrno proprietà stringa
                    else
                    {
                        var properties = ent.GetType().GetProperties();
                        if (properties.Any())
                        {
                            string fallbackValue = string.Empty;
                            foreach (var prop in properties)
                            {
                                var value = prop.GetValue(ent)?.ToString();
                                if (!string.IsNullOrWhiteSpace(value))
                                {
                                    valueToShow = value;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (!Values.ContainsKey(id))
                    Values.Add(id, valueToShow);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante la lettura dell'ID: {ex.Message}");
                continue;
            }

        }
    }


    private void PopulateComboDynamic(IEnumerable<DynamicEntryDto> entitiesList)
    {
        foreach (var ent in entitiesList)
        {
            string valueToShow = string.Empty;
            Guid id = ent.Id;

            try
            {
                if (ent.ExtraProperties != null && ent.ExtraProperties.ContainsKey("Name"))
                {
                    var nameValue = ent.ExtraProperties["Name"];
                    if (nameValue != null && !string.IsNullOrWhiteSpace(nameValue.ToString()))
                    {
                        valueToShow = nameValue.ToString();
                    }
                }

                if (string.IsNullOrEmpty(valueToShow) && ent.ExtraProperties != null)
                {
                    foreach (var kvp in ent.ExtraProperties)
                    {
                        if (kvp.Value != null)
                        {
                            string stringValue = kvp.Value.ToString();
                            if (!string.IsNullOrWhiteSpace(stringValue))
                            {
                                valueToShow = stringValue;
                                break;
                            }
                        }
                    }
                }

                if (string.IsNullOrEmpty(valueToShow))
                {
                    valueToShow = id.ToString();
                }

                if (!Values.ContainsKey(id))
                    Values.Add(id, valueToShow);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante la lettura dell'ID: {ex.Message}");
                continue;
            }
        }
    }


    [Parameter]
    public EventCallback<string> OnSelectChangedCallBack { get; set; }
    private async Task OnSelectChanged(Guid arg)
    {
        OnSelectChangedCallBack.InvokeAsync(System.Text.Json.JsonSerializer.Serialize(
            new { Id = arg, Value = Values[arg], Domain= Domain }));
    }
}