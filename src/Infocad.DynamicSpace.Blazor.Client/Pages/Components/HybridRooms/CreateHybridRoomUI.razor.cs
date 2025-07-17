using Blazorise;
using Infocad.DynamicSpace.DynamicAttributes;
using Infocad.DynamicSpace.DynamicEntities;
using Infocad.DynamicSpace.DynamicRules;
using Infocad.DynamicSpace.HybridRooms;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;

namespace Infocad.DynamicSpace.Blazor.Client.Pages.Components.HybridRooms
{
    public partial class CreateHybridRoomUI
    {
        [Parameter]
        public DynamicEntityDto Entity { get; set; }

        [Parameter]
        public List<DynamicAttributeDto> DynamicAttributes { get; set; }

        [Parameter]
        public List<DynamicRuleDto> DynamicRules { get; set; }

        [Parameter]
        public EventCallback<HybridRoomDto> Save { get; set; }

        [Parameter]
        public EventCallback Close { get; set; }

        private bool _disabled = false;
        private Validations _validations;
        private HybridRoomDto _hybridRoom;

        protected override async Task OnInitializedAsync()
        {
            await InitializeHybridRoom();
            await base.OnInitializedAsync();
        }

        protected override async Task OnParametersSetAsync()
        {
        }

        Task OnStatusChanged(ValidationsStatusChangedEventArgs e)
        {
            _disabled = e.Status == ValidationStatus.Error;
            StateHasChanged();
            return Task.CompletedTask;
        }

        private async Task InitializeHybridRoom()
        {
            try
            {
                _hybridRoom = new HybridRoomDto();
                _disabled = false;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore nell'inizializzazione della stanza ibrida: {ex.Message}");
                throw;
            }
        }

        public T GetValue<T>(string attributeName)
        {
            if (_hybridRoom.ExtraProperties.TryGetValue(attributeName, out var value))
                return (T)value;
            return default(T);
        }

        private async void SetValue<T>(string attributeName, T value)
        {
            if (value != null)
                _hybridRoom.SetProperty(attributeName, value);

        }

        private void ValidateNavigation(ValidatorEventArgs e)
        {
            var value = e.Value as Guid?;

            if(value != null)
            {
                e.Status = ValidationStatus.Success;
            }
            else
            {
                e.Status = ValidationStatus.Error;
            }
        }

        private async Task SaveEntry()
        {
            try
            {
                if (_validations != null)
                {
                    if (Save.HasDelegate && _hybridRoom != null)
                    {
                        await Save.InvokeAsync(_hybridRoom);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante il salvataggio: {ex.Message}");
                // Qui potresti mostrare un messaggio di errore all'utente
            }
        }

        private async Task CloseModal()
        {
            try
            {
                if (Close.HasDelegate)
                    await Close.InvokeAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante la chiusura: {ex.Message}");
            }
        }
    }
}
