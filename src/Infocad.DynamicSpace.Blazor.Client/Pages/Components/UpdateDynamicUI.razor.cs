using Blazorise;
using Infocad.DynamicSpace.DynamicAttributes;
using Infocad.DynamicSpace.DynamicEntities;
using Infocad.DynamicSpace.DynamicEntry;
using Infocad.DynamicSpace.DynamicRules;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using Volo.Abp.Data;

namespace Infocad.DynamicSpace.Blazor.Client.Pages.Components
{
    public partial class UpdateDynamicUI : DynamicSpaceComponentBase
    { 
        [Parameter]
        public DynamicEntityDto Entity { get; set; }
        [Parameter]
        public List<DynamicAttributeDto> DynamicAttributes { get; set; }
        [Parameter]
        public EventCallback<DynamicEntryDto> Edit { get; set; }
        [Parameter]
        public List<DynamicRuleDto> DynamicRules { get; set; }
        [Parameter]
        public ExpandoObject ExtraProperty { get; set; }
        [Parameter]
        public EventCallback Close { get; set; }
        private IDictionary<string, object> ExtraPropertyDict { get; set; }
        private bool _disabled = true;
        private Validations _validations;

        protected override async Task OnParametersSetAsync()
        {
            if (ExtraProperty != null)
            {
                ExtraPropertyDict = (IDictionary<string, object>)ExtraProperty;
            }
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

        Task OnStatusChanged(ValidationsStatusChangedEventArgs e)
        {
            _disabled = e.Status == ValidationStatus.Error;
            StateHasChanged();
            return Task.CompletedTask;
        }


        public T GetValue<T>(string attributeName)
        {

            if (ExtraPropertyDict.TryGetValue(attributeName, out var value))
                return (T)value;
            return default(T);
        }

        private async void SetValue<T>(string attributeName, T value)
        {
            if (value != null)
                ExtraPropertyDict[attributeName] = value;

        }

        private async Task EditEntry()
        {
            if (Edit.HasDelegate)
            {
                Guid entryId;
                ExtraPropertyDict.TryGetValue("EntryId", out var entryIdObj);
                entryId = (Guid)entryIdObj;

                ExtraPropertyDict.Remove("EntryId");

                DynamicEntryDto dynamicEntry = new DynamicEntryDto();
                dynamicEntry.Id = entryId;
                foreach (var dict in ExtraPropertyDict)
                {
                    dynamicEntry.SetProperty(dict.Key, dict.Value);
                }

                await Edit.InvokeAsync(dynamicEntry);
            }
        }

        private async Task CloseModal()
        {
            if (Close.HasDelegate)
                await Close.InvokeAsync();
        }
    }
}
