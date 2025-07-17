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
    public partial class UpdateHybridRoomsUI
    {
        [Parameter]
        public DynamicEntityDto Entity { get; set; }

        [Parameter]
        public List<DynamicAttributeDto> DynamicAttributes { get; set; }

        [Parameter]
        public List<DynamicRuleDto> DynamicRules { get; set; }

        [Parameter]
        public HybridRoomDto HybridRoom { get; set; }

        [Parameter]
        public EventCallback<HybridRoomDto> Edit { get; set; }

        [Parameter]
        public EventCallback Close { get; set; }

        private bool _disabled = false;
        private Validations _validations;
        private HybridRoomDto _editableRoom;
        private IDictionary<string, object> _extraPropertiesDict;

        protected override async Task OnInitializedAsync()
        {
            await InitializeEditableRoom();
            await base.OnInitializedAsync();
        }

        Task OnStatusChanged(ValidationsStatusChangedEventArgs e)
        {
            _disabled = e.Status == ValidationStatus.Error;
            StateHasChanged();
            return Task.CompletedTask;
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

        protected override async Task OnParametersSetAsync()
        {
            if (HybridRoom != null && (_editableRoom?.Id != HybridRoom.Id))
            {
                await InitializeEditableRoom();
            }
            await base.OnParametersSetAsync();
        }

        private async Task InitializeEditableRoom()
        {
            try
            {
                HybridRoomDto room = HybridRoom ?? new HybridRoomDto();

                _editableRoom = room.Clone();

                // Copia le ExtraProperties esistenti
                if (room.ExtraProperties != null)
                {
                    foreach (var kvp in room.ExtraProperties)
                    {
                        _editableRoom.ExtraProperties[kvp.Key] = kvp.Value;
                    }
                }

                _extraPropertiesDict = _editableRoom.ExtraProperties;
                _disabled = false;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore nell'inizializzazione della modifica stanza ibrida: {ex.Message}");
                throw;
            }
        }

        public T GetValue<T>(string attributeName)
        {
            if (_editableRoom.ExtraProperties.TryGetValue(attributeName, out var value))
                return (T)value;
            return default(T);
        }

        private async void SetValue<T>(string attributeName, T value)
        {
            if (value != null)
                _editableRoom.SetProperty(attributeName, value);
        }

        private async Task ValidateForm()
        {
            try
            {
                if (_validations != null)
                {
                    var isValid = await _validations.ValidateAll();
                    _disabled = !isValid;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante la validazione: {ex.Message}");
                _disabled = true;
            }
        }

        private async Task SaveChanges()
        {
            try
            {
                if (_validations != null)
                {
                    if (Edit.HasDelegate && _editableRoom != null)
                        await Edit.InvokeAsync(_editableRoom);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante il salvataggio delle modifiche: {ex.Message}");
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

        // Event handlers per i campi statici che triggereranno la validazione
        private async Task OnNameChanged(string value)
        {
            try
            {
                _editableRoom.Name = value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante il cambio del nome: {ex.Message}");
            }
        }

        private async Task OnCapacityChanged(int value)
        {
            try
            {
                _editableRoom.Capacity = value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante il cambio della capacità: {ex.Message}");
            }
        }

        private async Task OnDescriptionChanged(string value)
        {
            try
            {
                _editableRoom.Description = value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante il cambio della descrizione: {ex.Message}");
            }
        }

        // Metodi di utilità
        public bool HasChanges()
        {
            if (HybridRoom == null || _editableRoom == null)
                return false;

            // Controlla cambiamenti nelle proprietà base
            if (HybridRoom.Name != _editableRoom.Name ||
                HybridRoom.Capacity != _editableRoom.Capacity ||
                HybridRoom.Description != _editableRoom.Description)
                return true;

            // Controlla cambiamenti nelle ExtraProperties
            if (HybridRoom.ExtraProperties?.Count != _editableRoom.ExtraProperties?.Count)
                return true;

            if (HybridRoom.ExtraProperties != null && _editableRoom.ExtraProperties != null)
            {
                foreach (var kvp in HybridRoom.ExtraProperties)
                {
                    if (!_editableRoom.ExtraProperties.TryGetValue(kvp.Key, out var newValue) ||
                        !Equals(kvp.Value, newValue))
                        return true;
                }
            }

            return false;
        }

        public void ResetChanges()
        {
            _ = InitializeEditableRoom();
        }
    }
}
