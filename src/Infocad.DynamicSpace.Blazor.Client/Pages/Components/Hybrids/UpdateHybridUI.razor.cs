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
    public partial class UpdateHybridUI : DynamicSpaceComponentBase
    {
        [Parameter]
        public DynamicEntityDto Entity { get; set; }

        [Parameter]
        public List<DynamicAttributeDto> DynamicAttributes { get; set; }

        [Parameter]
        public List<DynamicRuleDto> DynamicRules { get; set; }

        [Parameter]
        public EventCallback<object> Update { get; set; }

        [Parameter]
        public EventCallback Close { get; set; }

        [Parameter]
        public string HybridTypeAssemblyQualifiedName { get; set; }

        [Parameter]
        public object ExistingData { get; set; }

        private object _existingData;

        private bool _disabled = true;
        private bool _isLoading = true;
        private bool _isLoaded = false;
        private Validations _validations;
        private List<PropertyInfo> _staticProperties;
        private Type _hybridType;
        private object _hybridInstance;
        private PropertyInfo _extraPropertiesProperty;

        protected override async Task OnParametersSetAsync()
        {
            dynamic obj = ExistingData;
            dynamic oldObj = _existingData;
            try
            {
                if (obj!=null && (oldObj == null || obj.Id != oldObj.Id))
                {
                    _existingData = ExistingData;
                    await InitializeHybridType();
                    await LoadExistingData();
                }
            }
            catch(Exception e)
            {

            }
           
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

                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore nell'inizializzazione del tipo ibrido: {ex.Message}");
                throw;
            }
        }

        private async Task LoadExistingData()
        {
            try
            {
                _isLoading = true;
                StateHasChanged();

                if (ExistingData == null || _hybridInstance == null)
                {
                    throw new ArgumentException("ExistingData è richiesto per l'aggiornamento");
                }

                var dict = (IDictionary<string, object>)ExistingData;

                // Carica l'ID se presente
                var idProperty = _hybridType.GetProperty("Id");
                if (idProperty != null && dict.TryGetValue("Id", out var idValue) && idValue is Guid id)
                {
                    idProperty.SetValue(_hybridInstance, id);
                }

                // Carica DynamicEntityId se presente
                var dynamicEntityIdProperty = _hybridType.GetProperty("DynamicEntityId");
                if (dynamicEntityIdProperty != null)
                {
                    if (dict.TryGetValue("DynamicEntityId", out var dynamicEntityIdValue) && dynamicEntityIdValue is Guid dynamicEntityId)
                    {
                        dynamicEntityIdProperty.SetValue(_hybridInstance, dynamicEntityId);
                    }
                    else if (Entity != null)
                    {
                        dynamicEntityIdProperty.SetValue(_hybridInstance, Entity.Id);
                    }
                }

                // Carica le proprietà statiche
                foreach (var property in _staticProperties)
                {
                    if (dict.TryGetValue(property.Name, out var value) && value != null)
                    {
                        try
                        {
                            // Gestione speciale per i tipi nullable
                            Type targetType = property.PropertyType;
                            Type underlyingType = Nullable.GetUnderlyingType(targetType);

                            if (underlyingType != null)
                            {
                                // È un tipo nullable
                                if (value.ToString() == string.Empty)
                                {
                                    property.SetValue(_hybridInstance, null);
                                }
                                else
                                {
                                    var convertedValue = Convert.ChangeType(value, underlyingType);
                                    property.SetValue(_hybridInstance, convertedValue);
                                }
                            }
                            else
                            {
                                // Non è nullable
                                var convertedValue = Convert.ChangeType(value, targetType);
                                property.SetValue(_hybridInstance, convertedValue);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Errore nella conversione della proprietà {property.Name}: {ex.Message}");
                            // Imposta il valore di default se la conversione fallisce
                            if (property.PropertyType.IsValueType && Nullable.GetUnderlyingType(property.PropertyType) == null)
                            {
                                property.SetValue(_hybridInstance, Activator.CreateInstance(property.PropertyType));
                            }
                            else
                            {
                                property.SetValue(_hybridInstance, null);
                            }
                        }
                    }
                }

                // Inizializza ExtraProperties se non esiste
                if (_extraPropertiesProperty != null)
                {
                    var extraProperties = _extraPropertiesProperty.GetValue(_hybridInstance) as Dictionary<string, object>;
                    if (extraProperties == null)
                    {
                        extraProperties = new Dictionary<string, object>();
                        _extraPropertiesProperty.SetValue(_hybridInstance, extraProperties);
                    }

                    // Carica le proprietà dinamiche negli ExtraProperties
                    if (Entity?.Attributes != null && DynamicAttributes != null)
                    {
                        foreach (var entityAttribute in Entity.Attributes)
                        {
                            var attribute = DynamicAttributes.FirstOrDefault(a => a.Id == entityAttribute.DynamicAttributeId);
                            if (attribute != null && dict.TryGetValue(attribute.Name, out var dynamicValue))
                            {
                                // Converti il valore al tipo appropriato in base al tipo di attributo
                                object convertedDynamicValue = ConvertDynamicValue(dynamicValue, attribute.Type);
                                extraProperties[attribute.Name] = convertedDynamicValue;
                            }
                        }
                    }
                }

                _disabled = false;
                _isLoaded = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore nel caricamento dei dati esistenti: {ex.Message}");
                throw;
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

        private object ConvertDynamicValue(object value, DynamicAttributeType attributeType)
        {
            if (value == null || value.ToString() == string.Empty)
                return null;

            try
            {
                return attributeType switch
                {
                    DynamicAttributeType.Text => value.ToString(),
                    DynamicAttributeType.Number => Convert.ToInt32(value),
                    DynamicAttributeType.Float => Convert.ToSingle(value),
                    DynamicAttributeType.Boolean => Convert.ToBoolean(value),
                    DynamicAttributeType.DateTime => Convert.ToDateTime(value),
                    _ => value
                };
            }
            catch
            {
                return value; // Restituisce il valore originale se la conversione fallisce
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

        private async void SetStaticValue<T>(string propertyName, T value)
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

                _disabled = !await _validations.ValidateAll();
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

        private async void SetDynamicValue<T>(string attributeName, T value)
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

                _disabled = !await _validations.ValidateAll();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore nell'impostazione della proprietà dinamica {attributeName}: {ex.Message}");
            }
        }

        private async Task UpdateEntry()
        {
            try
            {
                if (Update.HasDelegate)
                    await Update.InvokeAsync(_hybridInstance);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante l'aggiornamento: {ex.Message}");
            }
        }

        private async Task CloseModal()
        {
            if (Close.HasDelegate)
                await Close.InvokeAsync();
        }
    }
}