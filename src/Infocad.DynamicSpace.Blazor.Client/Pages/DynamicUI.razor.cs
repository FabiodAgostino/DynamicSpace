using Infocad.DynamicSpace.DynamicEntities;
using Infocad.DynamicSpace.DynamicEntries;
using Infocad.DynamicSpace.DynamicEntry;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infocad.DynamicSpace.DynamicAttributes;
using Volo.Abp.Application.Dtos;
using Blazorise;
using Volo.Abp.Data;
using System.Dynamic;
using Infocad.DynamicSpace.DynamicFormats;
using Infocad.DynamicSpace.DynamicRules;

namespace Infocad.DynamicSpace.Blazor.Client.Pages
{
    public partial class DynamicUI
    {

        private Modal CreateModal;
        private Modal EditModal;

        public DynamicEntityDto DynamicEntity { get; set; }
        public PagedResultDto<DynamicEntryDto> PagedEntries { get; set; } = new();
        public ExpandoObject EditExtraProperty { get; set; } = new();

        public List<DynamicAttributeDto> DynamicAttributes { get; set; } = new();
        public List<DynamicFormatDto> DynamicFormats { get; set; }
        public List<DynamicRuleDto> DynamicRules { get; set; }


        private Guid _oldIdEntiy { get; set; }
        public bool IsFirstLoad = true;

        private async Task OpenCreateModal()
        {
            await CreateModal.Show();
        }

        private async Task OpenEditModal(ExpandoObject extraProperty)
        {
            EditExtraProperty = extraProperty;
            StateHasChanged();
            await EditModal.Show();
        }


        protected override async Task OnParametersSetAsync()
        {
            if (Id != _oldIdEntiy)
            {
                _oldIdEntiy = Id;
                var result = await _entityService.GetByIdIncludeAttributeAsync(Id);

                if (result != null)
                {
                    DynamicEntity = result;

                    DynamicAttributes = await _attributeService.GetListByGuids(DynamicEntity.Attributes
                        .Select(x => x.DynamicAttributeId).ToList());
                    await GetEntries();
                    await GetFormats();
                    await GetRules();
                }

            }
        }

        public async Task GetEntries()
        {
            var input = new GetDynamicEntryListDto() { MaxResultCount = 100, Sorting = "Id" };
            var entries = await _entryService.GetListEntryByEntityAsync(Id, input);
            if (entries != null)
                PagedEntries = entries;
        }

        public async Task GetFormats()
        {
            var input = new PagedAndSortedResultRequestDto() { MaxResultCount = 100 };
            var formats = await _formatService.GetListAsync(input);
            DynamicFormats = new();
            if (formats != null)
                DynamicFormats = formats.Items.ToList();
            else
                DynamicFormats = new();

            StateHasChanged();
        }

        public async Task GetRules()
        {
            var input = new PagedAndSortedResultRequestDto() { MaxResultCount = 100 };
            var rules = await _ruleService.GetListAsync(input);
            DynamicRules = new();
            if (rules != null)
                DynamicRules = rules.Items.ToList();
            else
                DynamicRules = new();

            StateHasChanged();
        }

        public async Task SaveEntry(DynamicEntryDto dynamicEntry)
        {
            dynamicEntry.DynamicEntityId = DynamicEntity.Id;
            await _entryService.CreateAsync(dynamicEntry);
            await CreateModal.Hide();
            await GetEntries();
        }

        public async Task EditEntry(DynamicEntryDto dynamicEntry)
        {
            dynamicEntry.DynamicEntityId = DynamicEntity.Id;
            await _entryService.UpdateAsync(dynamicEntry.Id, dynamicEntry);
            await EditModal.Hide();
            await GetEntries();
        }

        public async Task DeleteEntry(Guid id)
        {
            await _entryService.DeleteAsync(id);
            await EditModal.Hide();
            await GetEntries();
        }

    }
}
