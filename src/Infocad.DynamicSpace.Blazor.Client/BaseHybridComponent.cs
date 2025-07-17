using Blazorise;
using Infocad.DynamicSpace.DynamicAttributes;
using Infocad.DynamicSpace.DynamicEntities;
using Infocad.DynamicSpace.DynamicFormats;
using Infocad.DynamicSpace.DynamicRules;
using Infocad.DynamicSpace.HybridBuildings;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Infocad.DynamicSpace.Blazor.Client
{
    public abstract class BaseHybridComponent<T> : DynamicSpaceComponentBase where T : class
    {
        [Parameter]
        public string TypeName { get; set; }
        public string OldTypeName;
        public bool IsLoading = true;
        public object? ServiceInstance;
        public IEnumerable<T> Items = new List<T>();
        public Type? DtoType { get; set; }
        public string _baseName => TypeName.EndsWith("Dto") ? TypeName.Substring(0, TypeName.Length - 3) : TypeName;
        public DynamicEntityDto DynamicEntity;
        public List<DynamicAttributeDto> DynamicAttributes;
        public List<DynamicFormatDto> DynamicFormats;
        public List<DynamicRuleDto> DynamicRules = new();
        public Modal CreateModal;
        public Modal EditModal;
        public T SelectedItem;

        [Inject]
        public IServiceProvider ServiceProvider { get; set; }
        [Inject]
        public IDynamicEntityService DynamicEntityService { get; set; }
        [Inject]
        public IDynamicAttributeService DynamicAttributeService { get; set; }
        [Inject]
        public IDynamicFormatService DynamicFormatService { get; set; }
        [Inject]
        public IDynamicRuleService DynamicRuleService { get; set; }
        public Assembly? DomainAssembly { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            DomainAssembly = typeof(HybridBuildingDto).Assembly;
            DtoType = FindTypeInAssembly(DomainAssembly, TypeName);
            if (DtoType == null)
                throw new Exception($"Tipo DTO non trovato per: {TypeName}");

        }

        public virtual async Task LoadComponent()
        {
            OldTypeName = TypeName;
            await GetDynamicEntityInfo();
            await LoadItemsAsync();
        }


        private async Task GetDynamicEntityInfo()
        {
            if (DtoType == null || DtoType.AssemblyQualifiedName == null)
                return;

            try
            {
                DynamicEntity = await DynamicEntityService.GetFullEntityByHybridEntity(DtoType.AssemblyQualifiedName);

                if (DynamicEntity != null)
                {
                    var attributeIds = DynamicEntity.Attributes.Select(a => a.DynamicAttributeId).ToList();
                    if (attributeIds.Any())
                    {
                        DynamicAttributes = await DynamicAttributeService.GetListByGuids(DynamicEntity.Attributes
                         .Select(x => x.DynamicAttributeId).ToList());

                        var formatIds = DynamicEntity.Attributes
                            .Where(a => a.DynamicFormatId.HasValue)
                            .Select(a => a.DynamicFormatId.Value)
                            .Distinct()
                            .ToList();

                        if (formatIds.Any())
                        {
                            var result = await DynamicFormatService.GetListAsync(new PagedAndSortedResultRequestDto
                            { MaxResultCount = 100 });
                            DynamicFormats = result.Items.ToList();
                        }

                        var resultRules = await DynamicRuleService.GetListAsync(new PagedAndSortedResultRequestDto() { MaxResultCount = 100 });
                        DynamicRules = resultRules.Items.ToList();
                    }
                }
                else
                {
                    DynamicEntity = new() { Attributes = new() };
                    DynamicAttributes = new();
                    DynamicFormats = new();
                    DynamicRules = new();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore in GetDynamicEntityInfo: {ex.Message}");
            }
        }

        public abstract Task LoadItemsAsync();
       


        public abstract Task UpdateEntity(T hybridInstance);

        public abstract Task SaveNewEntity(T hybridInstance);

        public abstract Task DeleteEntity(Guid entityId);
        

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

        public async Task OpenEditModal(T hybridInstance)
        {
            SelectedItem = hybridInstance;
            EditModal.Show();
        }
        private Type FindTypeInAssembly(Assembly assembly, string typeNamePart)
        {
            // Prima, controlliamo se c'è una corrispondenza esatta con il nome del tipo
            var allTypes = assembly.GetTypes();

            // Cerca prima per nome esatto
            var exactMatch = allTypes.FirstOrDefault(t => t.Name == typeNamePart);
            if (exactMatch != null)
                return exactMatch;

            // Se non c'è una corrispondenza esatta, cerca i tipi che contengono il nome parziale
            // e hanno un namespace che contiene "Hybrid" e il TypeName
            var matchingTypes = allTypes
                .Where(t => t.Name == typeNamePart &&
                            t.Namespace != null &&
                            t.Namespace.Contains("Hybrid") &&
                            t.Namespace.Contains(TypeName))
                .ToList();

            if (matchingTypes.Count == 1)
            {
                return matchingTypes[0];
            }
            else if (matchingTypes.Count > 1)
            {
                // Se troviamo più tipi corrispondenti, preferiamo quello nel namespace più specifico
                // Ordiniamo per lunghezza del namespace (i più specifici tendono ad essere più lunghi)
                return matchingTypes.OrderByDescending(t => t.Namespace.Length).First();
            }

            // Se ancora non abbiamo trovato nulla, cerchiamo per nome di tipo senza considerare il namespace
            return allTypes.FirstOrDefault(t => t.Name == typeNamePart);
        }


    }

}
