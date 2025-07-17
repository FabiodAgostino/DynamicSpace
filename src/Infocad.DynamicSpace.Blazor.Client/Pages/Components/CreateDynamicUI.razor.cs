using Blazorise;
using Infocad.DynamicSpace.DynamicAttributes;
using Infocad.DynamicSpace.DynamicEntities;
using Infocad.DynamicSpace.DynamicEntry;
using Infocad.DynamicSpace.DynamicRules;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Data;
namespace Infocad.DynamicSpace.Blazor.Client.Pages.Components;

public partial class CreateDynamicUI : DynamicSpaceComponentBase
{
    [Parameter]
    public DynamicEntityDto Entity { get; set; }
    [Parameter]
    public List<DynamicAttributeDto> DynamicAttributes { get; set; }
    [Parameter]
    public List<DynamicRuleDto> DynamicRules { get; set; }
    [Parameter]
    public EventCallback<DynamicEntryDto> Save { get; set; }
    [Parameter]
    public EventCallback Close { get; set; }
    private bool _disabled = true;
    private Validations _validations;

    private DynamicEntryDto DynamicEntry { get; set; } = new DynamicEntryDto();

    public T GetValue<T>(string attributeName)
    {
        if (DynamicEntry.ExtraProperties.TryGetValue(attributeName, out var value))
            return (T)value;
        return default(T);
    }

    private void ValidateNavigation(ValidatorEventArgs e)
    {
        var value = e.Value as Guid?;

        if (value != null)
        {
            e.Status = ValidationStatus.Success;
        }
        else
        {
            e.Status = ValidationStatus.Error;
        }
    }

    private async void SetValue<T>(string attributeName, T value)
    {
        if (value != null)
            DynamicEntry.SetProperty(attributeName, value);

    }

    Task OnStatusChanged(ValidationsStatusChangedEventArgs e)
    {
        _disabled = e.Status == ValidationStatus.Error;
        StateHasChanged();
        return Task.CompletedTask;
    }

    private async Task SaveEntry()
    {
        if (Save.HasDelegate)
            await Save.InvokeAsync(DynamicEntry);
    }

    private async Task CloseModal()
    {
        if (Close.HasDelegate)
            await Close.InvokeAsync();
    }
}