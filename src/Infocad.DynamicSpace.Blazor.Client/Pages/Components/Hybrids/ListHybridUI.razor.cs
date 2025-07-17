using Blazorise;
using Blazorise.DataGrid;
using Infocad.DynamicSpace.DynamicAttributes;
using Infocad.DynamicSpace.DynamicEntities;
using Infocad.DynamicSpace.DynamicEntry;
using Infocad.DynamicSpace.DynamicFormats;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Infocad.DynamicSpace.Blazor.Client.Pages.Components.Hybrids
{
    public partial class ListHybridUI : DynamicSpaceComponentBase
    {

        [Parameter]
        public DynamicEntityDto Entity { get; set; }
        [Parameter]
        public List<DynamicAttributeDto> Attributes { get; set; }
        [Parameter]
        public List<DynamicFormatDto> Formats { get; set; }
        [Parameter]
        public EventCallback CreateModal { get; set; }
        [Parameter]
        public EventCallback<object> EditModal { get; set; }
        [Parameter]
        public EventCallback<Guid> DeleteEntity { get; set; }
        [Parameter]
        public string BaseName { get; set; }
        [Parameter]
        public Type? DtoType { get; set; }

        private DataGrid<ExpandoObject> _grid;
        [Parameter]
        public IEnumerable<object> Items{ get; set; }
        private NavigationDetailModal _navigationModal;



        public async Task OpenCreateModal()
        {
            if (CreateModal.HasDelegate)
                await CreateModal.InvokeAsync();

        }

        public async Task OpenEditModal(object hybridInstance)
        {
            if (EditModal.HasDelegate)
                await EditModal.InvokeAsync((ExpandoObject)hybridInstance);
        }

        public List<ExpandoObject> ConvertToExpandoObjects(IEnumerable<object> listItems)
        {
            var result = new List<ExpandoObject>();

            if (listItems == null)
                return result;

            try
            {
                foreach (var item in listItems)
                {
                    var expando = new ExpandoObject();
                    var expandoDict = (IDictionary<string, object>)expando;

                    //proprietà base
                    foreach (var property in item.GetType().GetProperties()
                        .Where(p => !p.Name.Equals("ExtraProperties") && !p.Name.Equals("DynamicEntityId")))
                    {
                        var value = property.GetValue(item);
                        expandoDict.Add(property.Name, value ?? string.Empty);
                    }

                    //extra property
                    var extraPropertiesProperty = item.GetType().GetProperty("ExtraProperties");
                    if (extraPropertiesProperty != null)
                    {
                        var extraProperties = extraPropertiesProperty.GetValue(item) as Dictionary<string, object>;
                        if (extraProperties != null)
                        {
                            foreach (var kvp in extraProperties)
                            {
                                // Evita la duplicazione se la chiave esiste già
                                if (!expandoDict.ContainsKey(kvp.Key))
                                {
                                    expandoDict.Add(kvp.Key, kvp.Value ?? string.Empty);
                                }
                            }
                        }
                    }

                    result.Add(expando);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante la conversione in ExpandoObject: {ex.Message}");
            }

            return result;
        }


        public async Task ConfirmDelete(object entity)
        {
            var message = L["This item will be deleted"].ToString();
            bool confirmed = await MessageService.Confirm(message);
            Guid id = ((dynamic)entity).Id;
            if (confirmed && DeleteEntity.HasDelegate)
            {
                await DeleteEntity.InvokeAsync(id);
            }
        }

    }
}
