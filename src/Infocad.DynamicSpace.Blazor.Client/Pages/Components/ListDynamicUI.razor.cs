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

namespace Infocad.DynamicSpace.Blazor.Client.Pages.Components;

public partial class ListDynamicUI : DynamicSpaceComponentBase
{
    [Parameter]
    public DynamicEntityDto Entity { get; set; }
    [Parameter]
    public PagedResultDto<DynamicEntryDto> Entries { get; set; }
    [Parameter]
    public List<DynamicAttributeDto> Attributes { get; set; }
    [Parameter]
    public List<DynamicFormatDto> Formats { get; set; }
    [Parameter]
    public EventCallback CreateModal { get; set; }
    [Parameter]
    public EventCallback<ExpandoObject> EditModal { get; set; }
    [Parameter]
    public EventCallback<Guid> DeleteEntry { get; set; }

    private DataGrid<ExpandoObject> _grid { get; set; }
    private List<ExpandoObject> inMemoryData = new List<ExpandoObject>();
    private NavigationDetailModal _navigationModal;

    public async Task OpenCreateModal()
    {
        if (CreateModal.HasDelegate)
            await CreateModal.InvokeAsync();

    }

    public async Task OpenEditModal(ExpandoObject extraProperty)
    {
        if (EditModal.HasDelegate)
            await EditModal.InvokeAsync(extraProperty);
    }

    public async Task ConfirmDelete(ExpandoObject extraProperty)
    {
        var dict = (IDictionary<string, object>)extraProperty;
        Guid entryId;
        dict.TryGetValue("EntryId", out var entryIdObj);
        entryId = (Guid)entryIdObj;

        // Mostra la modale di conferma usando MessageService di Blazorise
        bool confirmed = false;
        var message = L["This item will be deleted"].ToString();
        confirmed = await MessageService.Confirm(message);

        if (confirmed && DeleteEntry.HasDelegate)
        {
            await DeleteEntry.InvokeAsync(entryId);
        }
    }

    private ExpandoObject convertToExpandoObject(DynamicEntryDto dto)
    {
        var eo = new ExpandoObject();
        var eoColl = (ICollection<KeyValuePair<string, object>>)eo;
        eoColl.Add(new KeyValuePair<string, object>("EntryId",dto.Id));
        foreach (var kvp in dto.ExtraProperties)
        {
            eoColl.Add(kvp);
        }

        dynamic eoDynamic = eo;

        return eoDynamic;
    }

    
}