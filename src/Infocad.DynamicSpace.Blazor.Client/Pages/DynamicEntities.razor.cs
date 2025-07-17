using AutoMapper;
using Blazorise;
using Infocad.DynamicSpace.Blazor.Client.Pages.Modals;
using Infocad.DynamicSpace.DynamicAttributes;
using Infocad.DynamicSpace.DynamicControls;
using Infocad.DynamicSpace.DynamicEntities;
using Infocad.DynamicSpace.DynamicEntityAttributes;
using Infocad.DynamicSpace.DynamicFormats;
using Infocad.DynamicSpace.DynamicRules;
using Infocad.DynamicSpace.DynamicTypes;
using Infocad.DynamicSpace.HybridBuildings;
using Infocad.DynamicSpace.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Infocad.DynamicSpace.Blazor.Client.Pages
{
    public partial class DynamicEntities
    {
        private List<DynamicEntityDto> Entities = new();
        private int TotalCount;
        private int PageSize = 10;
        private int CurrentPage = 1;
        private UpdateDynamicEntityDto EditingEntity = new();
        private UpdateDynamicEntityDto BaseEditingEntity = new();
        private CreateDynamicEntityDto NewEntity = new();
        private Modal CreateModal;
        private Modal EditModal;
        private Validations CreateValidationsRef;
        private Validations EditValidationsRef;
        private List<DynamicAttributeDto> SelectedAttributes = new();

        private List<DynamicAttributeDto> AvailableAttributes = new();
        private List<DynamicTypeDto> AvailableDynamicTypes = new();
        private List<DynamicFormatDto> AvailableDynamicFormats = new();
        private List<DynamicRuleDto> AvailableDynamicRules = new();
        private List<DynamicControlDto> AvailableDynamicControls = new();
        private bool CanCreateDynamicEntity { get; set; }
        private bool CanEditDynamicEntity { get; set; }
        private bool CanDeleteDynamicEntity { get; set; }

        private HybridEntityChoice _hybridEntityChoice { get; set; }
        private List<DynamicEntityDto> _hybridTypes = new List<DynamicEntityDto>();
        private DynamicEntityDto _selectedHybridType { get; set; }
        private string _selectedHybridTypeString { get; set; }
        private bool _disabledSelectHybridType = false;
        private List<string> HybridEntitiesUsed { get;set; } = new List<string>();

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await LoadDynamicFormats();
            await LoadDynamicTypes();
            await LoadAttributes();
            await LoadEntities();
            await LoadDynamicRules();
            await GetHybridTypes();
            await LoadDynamicControls();
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateDynamicEntity = await AuthorizationService
                .IsGrantedAsync(DynamicSpacePermissions.DynamicEntity.Create);
            CanEditDynamicEntity = await AuthorizationService
                        .IsGrantedAsync(DynamicSpacePermissions.DynamicEntity.Edit);

            CanDeleteDynamicEntity = await AuthorizationService
                .IsGrantedAsync(DynamicSpacePermissions.DynamicEntity.Delete);
        }


        private async Task LoadEntities()
        {
            try
            {
                var request = new GetDynamicEntityListDto
                {
                    MaxResultCount = PageSize,
                    SkipCount = (CurrentPage - 1) * PageSize
                };

                var result = await DynamicEntityService.GetListAsync(request);
                Entities = result.Items.ToList();
                TotalCount = (int)result.TotalCount;
            }
            catch (Exception ex)
            {
            }
        }

        Task OnSelectedValueChanged(string value, bool edit=false)
        {

            if(!String.IsNullOrEmpty(value))
                _selectedHybridType = _hybridTypes.FirstOrDefault(x => x != null && x.Name == value);
            else
                _selectedHybridType = null;

            _selectedHybridTypeString = value;
            if(_selectedHybridType != null)
            {
                if (edit)
                {
                    EditingEntity.IsHybrid = true;
                    EditingEntity.HybridTypeName = _selectedHybridType.HybridTypeName;

                    NewEntity.IsHybrid = false;
                    NewEntity.HybridTypeName = null;
                }
                else
                {
                    NewEntity.IsHybrid = true;
                    NewEntity.HybridTypeName = _selectedHybridType.HybridTypeName;

                    EditingEntity.IsHybrid = false;
                    EditingEntity.HybridTypeName = null;
                }
            }

            StateHasChanged();
            return Task.CompletedTask;
        }

        private void CloseModal(bool edit=false)
        {
            _selectedHybridType = null;
            _selectedHybridTypeString = String.Empty;
            if(edit)
                EditModal.Hide();
            else
                CreateModal.Hide();
        }


        private void OpenCreateModal()
        {
            NewEntity = new CreateDynamicEntityDto();
            CreateModal.Show();
        }

      public async Task GetHybridTypes()
        {
            var hybridEntityUsed = await DynamicEntityService.GetHybridEntitiesUsed();
            HybridEntitiesUsed = hybridEntityUsed.Where(x=> !String.IsNullOrEmpty(x.HybridTypeName))
                .Select(x=> x.HybridTypeName).ToList();
            var hybridTypes = await DynamicEntityService.GetHybridEntities();
            hybridTypes.Insert(0, null);

            _hybridTypes = hybridTypes;
        }

        private string GetTypeDisplayName(DynamicEntityDto? type)
        {
            if(type == null)
                return "Seleziona un'entità ibrida";
            string displayName = type.Name;
            if (displayName.StartsWith("Hybrid"))
                displayName = displayName.Substring("Hybrid".Length);
            // if (displayName.EndsWith("Dto"))
            //     displayName = displayName.Substring(0, displayName.Length - "Dto".Length);
            return displayName;
        }

        private async Task CreateEntityAsync()
        {
            if (await CreateValidationsRef.ValidateAll())
            {
                await DynamicEntityService.CreateAsync(NewEntity);
                await LoadEntities();
                await CreateModal.Hide();
                _selectedHybridType = null;
                _selectedHybridTypeString = String.Empty;
            }
        }

        private async Task OpenEditModal(DynamicEntityDto entity)
        {
            SelectHybridEntity(entity);
            EditingEntity = new UpdateDynamicEntityDto()
            {
                Attributes = entity.Attributes.Select(x =>
                    ObjectMapper.Map<DynamicEntityAttributeDto, CreateDynamicEntityAttributeDto>(x)).ToList(),
                Description = entity.Description, Id = entity.Id, Name = entity.Name,
                DynamicTypeId = entity.DynamicTypeId
            };
            BaseEditingEntity = EditingEntity.Clone();

            await EditModal.Show();
        }

        private void SelectHybridEntity(DynamicEntityDto entity)
        {
            if (entity.IsHybrid && !String.IsNullOrEmpty(entity.Name))
            {
                _selectedHybridType = entity;
                string fullTypeName = _selectedHybridType?.HybridTypeName ?? string.Empty;
                if(!String.IsNullOrEmpty(fullTypeName))
                {
                    Type type = Type.GetType(fullTypeName);
                    _selectedHybridTypeString = type.Name;
                    _disabledSelectHybridType = true;
                }
                else
                {
                    _selectedHybridTypeString = String.Empty;
                    _disabledSelectHybridType = false;
                }

                StateHasChanged();
            }
        }

        private async Task UpdateEntityAsync()
        {
            if (await EditValidationsRef.ValidateAll())
            {
                await DynamicEntityService.UpdateAsync(EditingEntity);
                await LoadEntities();
                await EditModal.Hide();
                _selectedHybridType = null;
                _selectedHybridTypeString = String.Empty;
            }
        }

        private async Task DeleteEntity(DynamicEntityDto entity)
        {
            await DynamicEntityService.DeleteAsync(entity.Id);
            await LoadEntities();
        }

        private async Task LoadDynamicTypes()
        {
            var result = await DynamicTypeService.GetListAsync(new PagedAndSortedResultRequestDto
                { MaxResultCount = 100 });
            AvailableDynamicTypes = result.Items.ToList();
        }

        private Task OnModalClosing(ModalClosingEventArgs e)
        {
            e.Cancel = e.CloseReason != CloseReason.UserClosing;

            return Task.CompletedTask;
        }

        private async Task LoadDynamicFormats()
        {
            var result = await DynamicFormatService.GetListAsync(new PagedAndSortedResultRequestDto
                { MaxResultCount = 100 });
            AvailableDynamicFormats = result.Items.ToList();
        }

        private async Task LoadDynamicRules()
        {
            var result = await DynamicRuleService.GetListAsync(new PagedAndSortedResultRequestDto
            { MaxResultCount = 100 });
            AvailableDynamicRules = result.Items.ToList();
        }

        private async Task LoadDynamicControls()
        {
            var result = await DynamicControlService.GetListAsync(new PagedAndSortedResultRequestDto
            { MaxResultCount = 100 });
            AvailableDynamicControls = result.Items.ToList();
        }

        private async Task RemoveEntityAttributeAsync(CreateDynamicEntityAttributeDto entity)
        {
            await DynamicEntityService.DeleteAttributeAsync(entity.DynamicEntityId, entity.DynamicAttributeId);
        }

        private async Task LoadAttributes()
        {
            SelectedAttributes.Clear();
            var result = await DynamicAttributeService.GetListAsync(new PagedAndSortedResultRequestDto
                { MaxResultCount = 100 });
            AvailableAttributes = result.Items.ToList();
        }

        private void ValidateTypeEntity(ValidatorEventArgs e)
        {
            try
            {
                var value = (Guid)e.Value;
                e.Status = value == Guid.Empty ? ValidationStatus.Error : ValidationStatus.Success;
            }
            catch (Exception ex)
            {
                e.Status = ValidationStatus.Error;
            }
        }
    }
}