using System;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Infocad.DynamicSpace.Blazor.Client.Pages
{
    public partial class HybridEntities : BaseHybridComponent<object>
    {
       
        protected override async Task OnParametersSetAsync()
        {
            if (TypeName != OldTypeName)
            {
                await base.OnParametersSetAsync();
                await InitializeServiceAsync();
                await LoadComponent();
            }

        }


        private async Task InitializeServiceAsync()
        {
            try
            {
                IsLoading = true;
                string serviceInterfaceName = $"I{_baseName}Service";
                Type? serviceType = DomainAssembly.GetType($"{DtoType.Namespace}.{serviceInterfaceName}");

                if (serviceType == null)
                    throw new Exception($"Tipo servizio non trovato per: {serviceInterfaceName}");

                var serviceInstance = ServiceProvider.GetService(serviceType);
                if (serviceInstance == null)
                    throw new Exception($"Impossibile creare un'istanza del servizio: {serviceInterfaceName}");

                ServiceInstance = serviceInstance;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante l'inizializzazione: {ex.Message}");
                throw;
            }
        }

        public override async Task LoadItemsAsync()
        {
            try
            {
                IsLoading = true;

                string getListMethodName = "GetListObjects";

                var method = ServiceInstance.GetType().GetMethod(getListMethodName);
                if (method == null)
                {
                    throw new Exception($"Metodo {getListMethodName} non trovato sul servizio {ServiceInstance.GetType().Name}");
                }

                var parameters = method.GetParameters();
                if (parameters.Length < 2)
                {
                    throw new Exception($"Il metodo {getListMethodName} non ha un parametro DTO");
                }

                Type listDtoType = parameters[1].ParameterType;

                var listDto = Activator.CreateInstance(listDtoType);

                var maxResultCountProperty = listDtoType.GetProperty("MaxResultCount");
                if (maxResultCountProperty != null && maxResultCountProperty.CanWrite)
                    maxResultCountProperty.SetValue(listDto, 100);

                var result = await InvokeServiceMethodAsync(getListMethodName, null, listDto);
                dynamic list = result;
                Items = list.Items;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore in LoadItemsAsync: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }
       

        private async Task<object> InvokeServiceMethodAsync(string methodName, params object[] objectParams)
        {
            if (ServiceInstance == null)
                throw new InvalidOperationException("Servizio non inizializzato.");

            var method = ServiceInstance.GetType().GetMethod(methodName);
            if (method == null)
                throw new InvalidOperationException($"Metodo '{methodName}' non trovato sul servizio.");

            var parameters = method.GetParameters();
            if (parameters.Length != objectParams.Count())
                throw new Exception($"Il metodo {methodName} deve avere esattamente {parameters.Count()} parametri DTO");

            var task = method.Invoke(ServiceInstance, objectParams);

            await (Task)task;

            var resultProperty = task.GetType().GetProperty("Result");
            if (resultProperty != null)
                return resultProperty.GetValue(task);

            return null;
        }

        public override async Task UpdateEntity(object hybridInstance)
        {
            if (hybridInstance != null)
            {
                try
                {
                    dynamic obj = hybridInstance;
                    IsLoading = true;
                    string createMethodName = "UpdateAsync";
                    var result = await InvokeServiceMethodAsync(createMethodName, obj.Id, hybridInstance);

                    if (result != null)
                        await LoadItemsAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Errore in CreateItemAsync: {ex.Message}");
                    throw;
                }
                finally
                {

                    IsLoading = false;
                    await CloseEditModal();
                    StateHasChanged();
                }

            }
        }

        public override async Task SaveNewEntity(object hybridInstance)
        {
            if (hybridInstance != null)
            {
                try
                {
                    IsLoading = true;
                    string createMethodName = "CreateAsync";
                    var result = await InvokeServiceMethodAsync(createMethodName, hybridInstance);

                    if (result != null)
                        await LoadItemsAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Errore in CreateItemAsync: {ex.Message}");
                    throw;
                }
                finally
                {
                    IsLoading = false;
                    await CloseCreateModal();
                    StateHasChanged();
                }

            }
        }
        public override async Task DeleteEntity(Guid entityId)
        {
            try
            {
                IsLoading = true;
                string createMethodName = "DeleteAsync";
                var result = await InvokeServiceMethodAsync(createMethodName, entityId);

                if (result != null)
                    await LoadItemsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore in CreateItemAsync: {ex.Message}");
                throw;
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        public async Task OpenCreateModal()
        {
            await CreateModal.Show();
        }

        public async Task CloseCreateModal()
        {
            await CreateModal.Hide();
        }

        public async Task CloseEditModal()
        {
            await EditModal.Hide();
        }

        public async Task OpenEditModal(ExpandoObject hybridInstance)
        {
            SelectedItem = hybridInstance;
            EditModal.Show();
        }

        
    }
}