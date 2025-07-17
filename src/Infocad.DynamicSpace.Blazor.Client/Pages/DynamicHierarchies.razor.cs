using Blazorise;
using Blazorise.DataGrid;
using Infocad.DynamicSpace.DynamicAttributes;
using Infocad.DynamicSpace.DynamicEntities;
using Infocad.DynamicSpace.DynamicHierarchies;
using Infocad.DynamicSpace.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.BlazoriseUI.Components.ObjectExtending;

namespace Infocad.DynamicSpace.Blazor.Client.Pages;

public partial class DynamicHierarchies
{
    private IReadOnlyList<DynamicHierarchyDto> Hierarchies;
    private int? TotalCount { get; set; }
    private int PageSize { get; set; } = 10;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; }

    public bool HasCreatePermission { get; set; }
    public bool HasUpdatePermission { get; set; }
    public bool HasDeletePermission { get; set; }

    private CreateDynamicHierarchyDto NewHierarchy { get; set; }
    private CreateDynamicHierarchyDto EditHierarchy { get; set; }
    private Modal CreateModal { get; set; }
    private Modal EditModal { get; set; }
    private Validations CreateValidationsRef;
    private Validations EditValidationsRef;
    private Guid EditingId { get; set; }

    public class NavigationItem
    {
        public Guid EntityId { get; set; }
        public string EntityName { get; set; }
    }

    private List<List<NavigationItem>> NavigationLevels = new List<List<NavigationItem>>();
    private List<Guid?> SelectedIds = new List<Guid?>();
    private List<bool> LoadingStates = new List<bool>();
    private List<DynamicEntityDto> AvailableRootEntities = new List<DynamicEntityDto>();
    private Guid? SelectedRootEntityId = null;
    private List<DynamicEntityDto> AllEntitiesCache = null;

    public DynamicHierarchies()
    {
        LocalizationResource = typeof(DynamicSpaceResource);
        NewHierarchy = new CreateDynamicHierarchyDto();
        EditHierarchy = new CreateDynamicHierarchyDto();
    }

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();
        await GetDynamicHierarchiesAsync();
        await InitializeNavigationAsync();
    }

    private async Task SetPermissionsAsync()
    {
        HasCreatePermission = await AuthorizationService.IsGrantedAsync("DynamicSpace.DynamicHierarchy.Create");
        HasUpdatePermission = await AuthorizationService.IsGrantedAsync("DynamicSpace.DynamicHierarchy.Edit");
        HasDeletePermission = await AuthorizationService.IsGrantedAsync("DynamicSpace.DynamicHierarchy.Delete");
    }

    private async Task InitializeNavigationAsync()
    {
        try
        {
            AllEntitiesCache = await DynamicEntityService.GetAllDynamicEntity();
            AvailableRootEntities = AllEntitiesCache ?? new List<DynamicEntityDto>();
        }
        catch
        {
            AllEntitiesCache = new List<DynamicEntityDto>();
            AvailableRootEntities = new List<DynamicEntityDto>();
        }

        NavigationLevels.Clear();
        SelectedIds.Clear();
        LoadingStates.Clear();
        SelectedRootEntityId = null;
        StateHasChanged();
    }

    private async Task<List<NavigationItem>> LoadNavigationItemsAsync(Guid targetEntityId)
    {
        try
        {
            var attributes = await DynamicAttributeService.GetNavChildEntities(targetEntityId);
            var navigationItems = new List<NavigationItem>();

            foreach (var attribute in attributes)
            {
                try
                {
                    var ownerEntity = await FindOwnerEntityAsync(attribute.Id);
                    if (ownerEntity != null)
                    {
                        navigationItems.Add(new NavigationItem
                        {
                            EntityId = ownerEntity.Id,
                            EntityName = ownerEntity.Name
                        });
                    }
                }
                catch
                {
                    continue;
                }
            }

            return navigationItems;
        }
        catch
        {
            return new List<NavigationItem>();
        }
    }

    private async Task<DynamicEntityDto> FindOwnerEntityAsync(Guid attributeId)
    {
        try
        {
            foreach (var entity in AllEntitiesCache)
            {
                var entityWithAttributes = await DynamicEntityService.GetByIdIncludeAttributeAsync(entity.Id);
                if (entityWithAttributes?.Attributes?.Any(a => a.DynamicAttributeId == attributeId) == true)
                {
                    return entity;
                }
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    private async Task OnRootEntityChanged(Guid? rootEntityId)
    {
        SelectedRootEntityId = rootEntityId;
        NavigationLevels.Clear();
        SelectedIds.Clear();
        LoadingStates.Clear();

        if (!rootEntityId.HasValue)
        {
            StateHasChanged();
            return;
        }

        try
        {
            LoadingStates.Add(true);
            StateHasChanged();

            var firstLevelItems = await LoadNavigationItemsAsync(rootEntityId.Value);

            NavigationLevels.Add(firstLevelItems);
            SelectedIds.Add(null);
            LoadingStates[0] = false;

            StateHasChanged();
        }
        catch
        {
            NavigationLevels.Clear();
            SelectedIds.Clear();
            LoadingStates.Clear();
            StateHasChanged();
        }
    }

    private bool IsReconstructing = false;

    private async Task OnNavigationSelectionChanged(Guid? selectedEntityId, int levelIndex)
    {
        try
        {
            SelectedIds[levelIndex] = selectedEntityId;
            RemoveLevelsAfter(levelIndex);

            if (!selectedEntityId.HasValue || IsReconstructing)
            {
                StateHasChanged();
                return;
            }

            // Solo se non stiamo ricostruendo, aggiungi nuovi livelli
            LoadingStates.Add(true);
            SelectedIds.Add(null);
            StateHasChanged();

            var nextLevelItems = await LoadNavigationItemsAsync(selectedEntityId.Value);

            NavigationLevels.Add(nextLevelItems);
            LoadingStates[LoadingStates.Count - 1] = false;

            if (!nextLevelItems.Any())
            {
                NavigationLevels.RemoveAt(NavigationLevels.Count - 1);
                SelectedIds.RemoveAt(SelectedIds.Count - 1);
                LoadingStates.RemoveAt(LoadingStates.Count - 1);
            }

            StateHasChanged();
        }
        catch
        {
            if (LoadingStates.Count > levelIndex + 1)
            {
                LoadingStates[LoadingStates.Count - 1] = false;
            }
            StateHasChanged();
        }
    }

    private void RemoveLevelsAfter(int levelIndex)
    {
        while (NavigationLevels.Count > levelIndex + 1)
        {
            NavigationLevels.RemoveAt(NavigationLevels.Count - 1);
            SelectedIds.RemoveAt(SelectedIds.Count - 1);
            LoadingStates.RemoveAt(LoadingStates.Count - 1);
        }
    }

    private async Task ResetNavigation()
    {
        await InitializeNavigationAsync();
    }

    private string GetNavigationBreadcrumb()
    {
        var breadcrumb = new List<string>();

        if (SelectedRootEntityId.HasValue)
        {
            breadcrumb.Add(GetSelectedRootEntityName());
        }

        for (int i = 0; i < NavigationLevels.Count; i++)
        {
            if (i < SelectedIds.Count && SelectedIds[i].HasValue)
            {
                var selectedItem = NavigationLevels[i]
                    .FirstOrDefault(item => item.EntityId == SelectedIds[i].Value);

                if (selectedItem != null)
                {
                    breadcrumb.Add(selectedItem.EntityName);
                }
            }
        }

        return string.Join(" → ", breadcrumb);
    }

    private Guid? GetLastSelectedEntityId()
    {
        for (int i = SelectedIds.Count - 1; i >= 0; i--)
        {
            if (SelectedIds[i].HasValue)
            {
                return SelectedIds[i].Value;
            }
        }

        return SelectedRootEntityId;
    }

    private string GetSelectedRootEntityName()
    {
        if (!SelectedRootEntityId.HasValue)
            return "";

        var rootEntity = AvailableRootEntities.FirstOrDefault(e => e.Id == SelectedRootEntityId.Value);
        return rootEntity?.Name ?? "Entità sconosciuta";
    }

    private async Task GetDynamicHierarchiesAsync()
    {
        var result = await DynamicHierarchyService.GetListAsync(new GetDyanmicHierarchyListDto()
        {
            MaxResultCount = PageSize,
            SkipCount = CurrentPage * PageSize,
            Sorting = CurrentSorting
        });
        Hierarchies = result.Items;
        TotalCount = (int)result.TotalCount;
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<DynamicHierarchyDto> e)
    {
        CurrentSorting = e.Columns
            .Where(c => c.SortDirection != SortDirection.Default)
            .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
            .JoinAsString(",");
        CurrentPage = e.Page - 1;

        await GetDynamicHierarchiesAsync();
        await InvokeAsync(StateHasChanged);
    }

    private void OpenCreateModal()
    {
        CreateValidationsRef.ClearAll();
        NewHierarchy = new CreateDynamicHierarchyDto();
        NavigationLevels.Clear();
        SelectedIds.Clear();
        LoadingStates.Clear();
        CreateModal.Show();
    }

    private void CloseCreateModal()
    {
        CreateModal.Hide();
    }

    private async Task CreateHierarchyAsync()
    {
        try
        {
            if (await CreateValidationsRef.ValidateAll())
            {
                var hierarchy = await DynamicHierarchyService.CreateAsync(NewHierarchy);

                await SaveNavigationRelationsAsync(hierarchy.Id);

                Hierarchies = Hierarchies.Append(hierarchy).ToList();
                TotalCount = (TotalCount ?? 0) + 1;
                CreateModal.Hide();
                NewHierarchy = new CreateDynamicHierarchyDto();
            }
        }
        catch (Exception e)
        {
            await HandleErrorAsync(e);
        }
    }

    private async Task SaveNavigationRelationsAsync(Guid hierarchyId)
    {
        try
        {
            if (!SelectedRootEntityId.HasValue) return;

            Guid? previousEntityId = SelectedRootEntityId;
            var rootEntity = new CreateDynamicHiererchyEntityDto
            {
                DynamicSourceEntityId = null,
                DynamicTargetEntityId = previousEntityId.Value,
                DisplayFields = AvailableRootEntities.SingleOrDefault(x => x.Id == previousEntityId.Value).Name
            };
            await DynamicHierarchyService.CreateDynamicHiererchyEntityAsync(hierarchyId, rootEntity);

            for (int i = 0; i < SelectedIds.Count; i++)
            {
                if (SelectedIds[i].HasValue)
                {
                    var selectedItem = NavigationLevels[i]
                        .FirstOrDefault(item => item.EntityId == SelectedIds[i].Value);

                    if (selectedItem != null && previousEntityId.HasValue)
                    {
                        var relationInput = new CreateDynamicHiererchyEntityDto
                        {
                            DynamicSourceEntityId = previousEntityId.Value,
                            DynamicTargetEntityId = selectedItem.EntityId,
                            DisplayFields = selectedItem.EntityName
                        };

                        await DynamicHierarchyService.CreateDynamicHiererchyEntityAsync(hierarchyId, relationInput);

                        previousEntityId = selectedItem.EntityId;
                    }
                }
            }
        }
        catch
        {
        }
    }

    private async void OpenEditModal(DynamicHierarchyDto hierarchy)
    {
        EditValidationsRef.ClearAll();
        EditingId = hierarchy.Id;
        EditHierarchy = ObjectMapper.Map<DynamicHierarchyDto, CreateDynamicHierarchyDto>(hierarchy);
        await LoadExistingHierarchyNavigationAsync(hierarchy.Id);
        
        EditModal.Show();
    }

    private void CloseEditModal()
    {
        EditModal.Hide();
    }

    private async Task UpdateHierarchyAsync(MouseEventArgs arg)
    {
        try
        {
            if (await EditValidationsRef.ValidateAll())
            {
                await DynamicHierarchyService.UpdateAsync(EditingId, EditHierarchy);
                await DynamicHierarchyService.DeleteAllHierarchyEntityAsync(EditingId);
                await SaveNavigationRelationsAsync(EditingId);

                await GetDynamicHierarchiesAsync();
                EditModal.Hide();
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task LoadExistingHierarchyNavigationAsync(Guid hierarchyId)
    {
        try
        {
            // Reset della navigazione
            await InitializeNavigationAsync();
            // Carica le relazioni esistenti
            var hierarchyEntities = await DynamicHierarchyService.GetDynamicHierarchyEntitiesAsync(hierarchyId);

            if (!hierarchyEntities.Any()) return;

            // Ricostruisci la catena di navigazione
            await ReconstructNavigationChainAsync(hierarchyEntities);
        }
        catch
        {
            // Se fallisce, lascia la navigazione vuota
            await InitializeNavigationAsync();
        }
    }

    private async Task ReconstructNavigationChainAsync(List<DynamicHierarchyEntityDto> hierarchyEntities)
    {
        IsReconstructing = true;

        try
        {
            // Trova l'entità root
            var rootEntityId = hierarchyEntities.FirstOrDefault(x => x.DynamicSourceEntityId == null)?.DynamicTargetEntityId;

            SelectedRootEntityId = rootEntityId;
            NavigationLevels.Clear();
            SelectedIds.Clear();
            LoadingStates.Clear();

            if (!rootEntityId.HasValue)
            {
                IsReconstructing = false;
                StateHasChanged();
                return;
            }

            var firstLevelItems = await LoadNavigationItemsAsync(rootEntityId.Value);
            NavigationLevels.Add(firstLevelItems);
            SelectedIds.Add(null);
            LoadingStates.Add(false);

            // Ricostruisci la catena seguendo le relazioni
            Guid? currentEntityId = rootEntityId;
            int levelIndex = 0;

            while (currentEntityId.HasValue && levelIndex < NavigationLevels.Count)
            {
                var nextRelation = hierarchyEntities.FirstOrDefault(e => e.DynamicSourceEntityId == currentEntityId.Value);
                if (nextRelation == null) break;

                // Imposta la selezione per il livello corrente
                SelectedIds[levelIndex] = nextRelation.DynamicTargetEntityId;

                // Carica il livello successivo se esiste
                var nextLevelItems = await LoadNavigationItemsAsync(nextRelation.DynamicTargetEntityId);
                if (nextLevelItems.Any())
                {
                    NavigationLevels.Add(nextLevelItems);
                    SelectedIds.Add(null);
                    LoadingStates.Add(false);
                }

                currentEntityId = nextRelation.DynamicTargetEntityId;
                levelIndex++;
            }
        }
        catch (Exception ex)
        {
            // In caso di errore, reset
            await InitializeNavigationAsync();
        }
        finally
        {
            IsReconstructing = false;
            StateHasChanged();
        }
    }

    private async Task RemoveLevelAndAfter(int levelIndex)
    {
        while (NavigationLevels.Count > levelIndex)
        {
            NavigationLevels.RemoveAt(NavigationLevels.Count - 1);
            SelectedIds.RemoveAt(SelectedIds.Count - 1);
            LoadingStates.RemoveAt(LoadingStates.Count - 1);
        }

        StateHasChanged();
    }

    private Guid? FindRootEntity(List<DynamicHierarchyEntityDto> hierarchyEntities)
    {
        var allTargetIds = hierarchyEntities.Select(e => e.DynamicTargetEntityId).ToHashSet();
        return hierarchyEntities
            .Select(e => e.DynamicSourceEntityId)
            .FirstOrDefault(sourceId => !allTargetIds.Contains(sourceId.Value));
    }

    private async Task DeleteEntity(DynamicHierarchyDto context)
    {
        try
        {
            var confirmMessage = L["HierarchyDeletionConfirmationMessage", context.Name];
            if (!await Message.Confirm(confirmMessage))
            {
                return;
            }

            await DynamicHierarchyService.DeleteAsync(context.Id);
            await GetDynamicHierarchiesAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task AddNewNavigationLevel()
    {
        if (!CanAddNewLevel()) return;

        var lastSelectedId = GetLastSelectedEntityId();
        if (!lastSelectedId.HasValue) return;

        try
        {
            LoadingStates.Add(true);
            SelectedIds.Add(null);
            StateHasChanged();

            var nextLevelItems = await LoadNavigationItemsAsync(lastSelectedId.Value);
            NavigationLevels.Add(nextLevelItems);
            LoadingStates[LoadingStates.Count - 1] = false;

            StateHasChanged();
        }
        catch
        {
            // Rimuovi il livello fallito
            if (LoadingStates.Count > 0)
                LoadingStates.RemoveAt(LoadingStates.Count - 1);
            if (SelectedIds.Count > 0)
                SelectedIds.RemoveAt(SelectedIds.Count - 1);

            StateHasChanged();
        }
    }

    private bool CanAddNewLevel()
    {
        if (!SelectedRootEntityId.HasValue) return false;

        // Verifica che l'ultimo livello abbia una selezione
        var lastSelectedId = GetLastSelectedEntityId();
        return lastSelectedId.HasValue;
    }

    private async Task ClearRootSelection()
    {
        await OnRootEntityChanged(null);
    }
}