using Blazorise;
using Infocad.DynamicSpace.DynamicAttributes;
using Infocad.DynamicSpace.DynamicEntities;
using Infocad.DynamicSpace.DynamicRules;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Volo.Abp.Data;

namespace Infocad.DynamicSpace.Blazor.Client.Pages.Components.Hybrids
{
    public partial class CreateHybridUI : DynamicSpaceComponentBase
    {
        [Parameter]
        public DynamicEntityDto Entity { get; set; }

        [Parameter]
        public List<DynamicAttributeDto> DynamicAttributes { get; set; }

        [Parameter]
        public List<DynamicRuleDto> DynamicRules { get; set; }

        [Parameter]
        public EventCallback<object> Save { get; set; }

        [Parameter]
        public EventCallback Close { get; set; }

        [Parameter]
        public string HybridTypeAssemblyQualifiedName { get; set; }
        private string _oldHybridTypeAssemblyQualifiedName { get; set; }


        private bool _disabled = true;
        private Validations _validations;
        private List<PropertyInfo> _staticProperties;
        private Type _hybridType;
        private object _hybridInstance;
        private PropertyInfo _extraPropertiesProperty;


        protected override async Task OnParametersSetAsync()
        {
            if(!string.IsNullOrEmpty(HybridTypeAssemblyQualifiedName) && HybridTypeAssemblyQualifiedName != _oldHybridTypeAssemblyQualifiedName)
            {
                _oldHybridTypeAssemblyQualifiedName = HybridTypeAssemblyQualifiedName;
                await InitializeHybridType();
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
        private async Task InitializeHybridType()
        {
            try
            {
                if (string.IsNullOrEmpty(HybridTypeAssemblyQualifiedName))
                {
                    throw new ArgumentException("HybridTypeAssemblyQualifiedName è richiesto");
                }

                // Ottieni il tipo dalla stringa AssemblyQualifiedName
                _hybridType = Type.GetType(HybridTypeAssemblyQualifiedName);
                if (_hybridType == null)
                {
                    throw new InvalidOperationException($"Impossibile trovare il tipo: {HybridTypeAssemblyQualifiedName}");
                }

                // Crea un'istanza del tipo
                _hybridInstance = Activator.CreateInstance(_hybridType);

                // Ottieni la proprietà ExtraProperties
                _extraPropertiesProperty = _hybridType.GetProperty("ExtraProperties");

                // Ottieni le proprietà statiche (escludi quelle ereditate da base e ExtraProperties)
                _staticProperties = _hybridType
                    .GetProperties()
                    .Where(p => p.CanWrite &&
                               !p.Name.Equals("Id") &&
                               !p.Name.Equals("ExtraProperties") &&
                               !p.Name.Equals("DynamicEntityId") &&
                               p.DeclaringType == _hybridType)
                    .ToList();

                // Imposta il DynamicEntityId se presente
                var dynamicEntityIdProperty = _hybridType.GetProperty("DynamicEntityId");
                if (dynamicEntityIdProperty != null && Entity != null)
                {
                    dynamicEntityIdProperty.SetValue(_hybridInstance, Entity.Id);
                }

                _disabled = false;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore nell'inizializzazione del tipo ibrido: {ex.Message}");
                throw;
            }
        }

        public T GetStaticValue<T>(string propertyName)
        {
            try
            {
                var property = _staticProperties.FirstOrDefault(p => p.Name == propertyName);
                if (property != null)
                {
                    var value = property.GetValue(_hybridInstance);
                    if (value == null)
                        return default(T);

                    if (value is T directValue)
                        return directValue;

                    // Prova a convertire il tipo se necessario
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                return default(T);
            }
            catch
            {
                return default(T);
            }
        }

        private async Task SetStaticValue<T>(string propertyName, T value)
        {
            try
            {
                var property = _staticProperties.FirstOrDefault(p => p.Name == propertyName);
                if (property != null)
                {
                    if (value != null)
                    {
                        var convertedValue = Convert.ChangeType(value, property.PropertyType);
                        property.SetValue(_hybridInstance, convertedValue);
                    }
                    else
                    {
                        property.SetValue(_hybridInstance, null);
                    }
                }

                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore nell'impostazione della proprietà statica {propertyName}: {ex.Message}");
            }
        }

        public T GetDynamicValue<T>(string attributeName)
        {
            try
            {
                if (_extraPropertiesProperty == null)
                    return default(T);

                var extraProperties = _extraPropertiesProperty.GetValue(_hybridInstance) as Dictionary<string, object>;
                if (extraProperties != null && extraProperties.TryGetValue(attributeName, out var value))
                {
                    if (value == null)
                        return default(T);

                    if (value is T directValue)
                        return directValue;

                    // Prova a convertire il tipo se necessario
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                return default(T);
            }
            catch
            {
                return default(T);
            }
        }

        private async Task SetDynamicValue<T>(string attributeName, T value)
        {
            try
            {
                if (_extraPropertiesProperty == null)
                    return;

                var extraProperties = _extraPropertiesProperty.GetValue(_hybridInstance) as Dictionary<string, object>;
                if (extraProperties == null)
                {
                    extraProperties = new Dictionary<string, object>();
                    _extraPropertiesProperty.SetValue(_hybridInstance, extraProperties);
                }

                if (value != null)
                {
                    extraProperties[attributeName] = value;
                }
                else
                {
                    extraProperties.Remove(attributeName);
                }

                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore nell'impostazione della proprietà dinamica {attributeName}: {ex.Message}");
            }
        }

        private async Task SaveEntry()
        {
            try
            {
                if (Save.HasDelegate)
                    await Save.InvokeAsync(_hybridInstance);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante il salvataggio: {ex.Message}");
            }
        }

        private async Task CloseModal()
        {

            if (Close.HasDelegate)
                await Close.InvokeAsync();
        }
    }
}