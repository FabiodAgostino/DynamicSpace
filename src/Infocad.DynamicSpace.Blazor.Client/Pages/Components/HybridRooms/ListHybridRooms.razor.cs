using Blazorise;
using Blazorise.DataGrid;
using Infocad.DynamicSpace.DynamicAttributes;
using Infocad.DynamicSpace.DynamicEntities;
using Infocad.DynamicSpace.DynamicFormats;
using Infocad.DynamicSpace.HybridRooms;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Infocad.DynamicSpace.Blazor.Client.Pages.Components.HybridRooms
{
    public partial class ListHybridRooms : DynamicSpaceComponentBase
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
        public EventCallback<HybridRoomDto> EditModal { get; set; }
        [Parameter]
        public EventCallback<Guid> DeleteEntity { get; set; }
        [Parameter]
        public string BaseName { get; set; }
        [Parameter]
        public Type? DtoType { get; set; }

        private DataGrid<ExpandoObject> _grid;
        [Parameter]
        public IEnumerable<HybridRoomDto> Items { get; set; }

        private List<ExpandoObject> _expandoItems { get; set; }
        private NavigationDetailModal _navigationModal;
        public async Task OpenCreateModal()
        {
            if (CreateModal.HasDelegate)
                await CreateModal.InvokeAsync();

        }

        public async Task OpenEditModal(HybridRoomDto hybridInstance)
        {
            if (EditModal.HasDelegate)
                await EditModal.InvokeAsync(hybridInstance);
        }


        public async Task ConfirmDelete(HybridRoomDto entity)
        {
            var message = L["This item will be deleted"].ToString();
            bool confirmed = await MessageService.Confirm(message);
            Guid id = ((dynamic)entity).Id;
            if (confirmed && DeleteEntity.HasDelegate)
            {
                await DeleteEntity.InvokeAsync(id);
            }
        }

        public ExpandoObject ConvertExtensibleToExpando(HybridRoomDto item)
        {
            var expando = new ExpandoObject();
            var expandoDict = (IDictionary<string, object>)expando;

            if (item == null)
                return expando;

            try
            {
                if (item.ExtraProperties != null)
                {
                    foreach (var kvp in item.ExtraProperties)
                    {
                        expandoDict[kvp.Key] = kvp.Value ?? string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante la conversione in ExpandoObject: {ex.Message}");
            }

            return expando;
        }

    }


}
