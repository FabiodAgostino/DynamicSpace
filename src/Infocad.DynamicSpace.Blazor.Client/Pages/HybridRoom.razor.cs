using Infocad.DynamicSpace.HybridRooms;
using System;
using System.Threading.Tasks;

namespace Infocad.DynamicSpace.Blazor.Client.Pages
{
    public partial class HybridRoom : BaseHybridComponent<HybridRoomDto>
    {
        protected override async Task OnParametersSetAsync()
        {
            if (TypeName != OldTypeName)
            {
                await base.OnParametersSetAsync();
                await LoadComponent();
            }

        }

        public override async Task LoadItemsAsync()
        {
            try
            {
                IsLoading = true;
                var result = await HybridRoomService.GetListObjects(null, new GetHybridRoomListDto() { MaxResultCount = 100 });
                Items = result.Items;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante il caricamento degli elementi: {ex.Message}");
                throw;
            }
            finally
            {
                IsLoading = false;
            }
        }

        public override async Task UpdateEntity(HybridRoomDto hybridInstance)
        {
            try
            {
                IsLoading = true;
                await HybridRoomService.UpdateAsync(hybridInstance.Id, hybridInstance);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante l'aggiornamento dell'istanza: {ex.Message}");
                throw;
            }
            finally
            {
                await CloseEditModal();
                await LoadItemsAsync();
                IsLoading = false;
            }
        }

        public override async Task SaveNewEntity(HybridRoomDto hybridInstance)
        {
            try
            {
                IsLoading = true;
                await HybridRoomService.CreateAsync(hybridInstance);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante il salvataggio della nuova istanza: {ex.Message}");
                throw;
            }
            finally
            {
                await CloseCreateModal();
                await LoadItemsAsync();
                IsLoading = false;
                StateHasChanged();
            }

        }

        public override async Task DeleteEntity(Guid entityId)
        {
            try
            {
                IsLoading = true;
                await HybridRoomService.DeleteAsync(entityId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante l'eliminazione dell'istanza: {ex.Message}");
                throw;
            }
            finally
            {
                await LoadItemsAsync();
                IsLoading = false;
                StateHasChanged();
            }
        }
    }
}
