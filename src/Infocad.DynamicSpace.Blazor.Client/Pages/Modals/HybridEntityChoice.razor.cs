using Blazorise;
using Infocad.DynamicSpace.HybridBuildings;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infocad.DynamicSpace.Blazor.Client.Pages.Modals
{
    partial class HybridEntityChoice : DynamicSpaceComponentBase
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private IServiceProvider ServiceProvider { get; set; }

        private Modal _modal { get; set; }
        private List<Type> _hybridTypes = new List<Type>();

        protected override void OnInitialized()
        {
            base.OnInitialized();
            LoadHybridTypes();
        }

        private void LoadHybridTypes()
        {
            // Referenzia specificamente l'assembly del dominio
            var domainAssembly = typeof(HybridBuildingDto).Assembly;
            var hybridTypes = domainAssembly.GetTypes()
                .Where(t => t.Name.StartsWith("Hybrid") &&
                            t.IsClass &&
                            !t.IsAbstract)
                .ToList();
            _hybridTypes = hybridTypes;
        }

        private string GetTypeDisplayName(Type? type)
        {
            string displayName = type.Name;
            if (displayName.StartsWith("Hybrid"))
                displayName = displayName.Substring("Hybrid".Length);
            if (displayName.EndsWith("Dto"))
                displayName = displayName.Substring(0, displayName.Length - "Dto".Length);
            return displayName;
        }

        private void NavigateToEntityPage(Type hybridType)
        {
            // Ottieni il nome del tipo senza prefisso "Hybrid" e senza suffisso "Dto"
            string typeName = GetTypeDisplayName(hybridType);

            // Reindirizza alla pagina HybridEntities con il parametro del tipo
            NavigationManager.NavigateTo($"/hybrid-entities/{typeName}");

            // Chiudi il modal dopo la navigazione
            CloseModal();
        }

        public void OpenModal()
        {
            _modal.Show();
        }

        private void CloseModal()
        {
            _modal.Hide();
        }
    }
}