using Infocad.DynamicSpace.DynamicEntities;
using Infocad.DynamicSpace.DynamicTypes;
using Infocad.DynamicSpace.Localization;
using Infocad.DynamicSpace.MultiTenancy;
using Infocad.DynamicSpace.Permissions;
using Localization.Resources.AbpUi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Volo.Abp.Account.Localization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity.Blazor;
using Volo.Abp.Identity.Settings;
using Volo.Abp.SettingManagement.Blazor.Menus;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.Blazor.Navigation;
using Volo.Abp.UI.Navigation;
using Volo.Abp.Users;

namespace Infocad.DynamicSpace.Blazor.Client.Navigation;

public class DynamicSpaceMenuContributor : IMenuContributor
{
    private readonly IConfiguration _configuration;

    public DynamicSpaceMenuContributor(IConfiguration configuration)
    {
        _configuration = configuration;
    }


    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
            await ConfigureDynamicMenu(context);
            await ConfigureHybridMenu(context); 
        }

        else if (context.Menu.Name == StandardMenus.User)
        {
            await ConfigureUserMenuAsync(context);
        }
    }

    private static async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<DynamicSpaceResource>();

        //Administration
        var administration = context.Menu.GetAdministration();
        administration.Order = 6;

        context.Menu.AddItem(new ApplicationMenuItem(
            DynamicSpaceMenus.Home,
            l["Menu:Home"],
            "/",
            icon: "fas fa-home",
            order: 1
        ));

        var dynamicSpaceMenu = new ApplicationMenuItem("Dynamic Space", l["Menu:DynamicSpace"], icon: "fa fa-map");
        context.Menu.AddItem(dynamicSpaceMenu);

        dynamicSpaceMenu.AddItem(new ApplicationMenuItem("Dynamic Types",
            l["Menu:DynamicType"], url: "/dynamictypes"))
            .RequirePermissions(DynamicSpacePermissions.DynamicType.Default);

        dynamicSpaceMenu.AddItem(new ApplicationMenuItem("Dynamic Formats",
            l["Menu:DynamicFormat"], url: "/dynamicformats"))
            .RequirePermissions(DynamicSpacePermissions.DynamicFormat.Default);

        dynamicSpaceMenu.AddItem(new ApplicationMenuItem("Dynamic Rules",
            l["Menu:DynamicRule"], url: "/dynamicrules"))
            .RequirePermissions(DynamicSpacePermissions.DynamicRule.Default);


        dynamicSpaceMenu.AddItem(new ApplicationMenuItem("Dynamic Attributes",
            l["Menu:DynamicAttribute"], url: "/dynamicattributes"));

        dynamicSpaceMenu.AddItem(new ApplicationMenuItem("Dynamic Entities",
            l["Menu:DynamicEntity"], url: "/dynamicentities"));
        
        dynamicSpaceMenu.AddItem(new ApplicationMenuItem("Dynamic Hierarchies",
            l["Menu:DynamicHierarchy"], url: "/dynamichierarchies"));


        dynamicSpaceMenu.AddItem(new ApplicationMenuItem("Dynamic Controls",
            l["Menu:DynamicControl"], url: "/dynamiccontrols"));

        dynamicSpaceMenu.AddItem(new ApplicationMenuItem("FeatureManagement",
           l["Menu:FeatureManagement"], url: "/feature-management"))
           .RequirePermissions(DynamicSpacePermissions.Feature.Default);

        dynamicSpaceMenu.AddItem(new ApplicationMenuItem("BlobStoring",
           l["Menu:BlobStoring"], url: "/file-management"))
           .RequirePermissions(DynamicSpacePermissions.Feature.Default);


        if (MultiTenancyConsts.IsEnabled)
        {
            administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);
        }
        else
        {
            administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
        }

        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 2);
        administration.SetSubItemOrder(SettingManagementMenus.GroupName, 3);

    }

    private async Task ConfigureDynamicMenu(MenuConfigurationContext context)
    {
        AbpAuthorizationService auth = (AbpAuthorizationService)context.AuthorizationService;
        if (auth.CurrentPrincipal.Identity.IsAuthenticated)
        {
            var _dynamicTypeService = context.ServiceProvider.GetRequiredService<IDynamicTypeService>();
            var _dynamicEntityService = context.ServiceProvider.GetRequiredService<IDynamicEntityService>();


            var l = context.GetLocalizer<DynamicSpaceResource>();
            var types = await _dynamicTypeService.GetListAsync(new PagedAndSortedResultRequestDto());

            foreach (var type in types.Items)
            {
                var menu = new ApplicationMenuItem(type.Name, l[$"Menu:{type.Name}"], icon: "fa fa-map")
                    .RequireAuthenticated();
                context.Menu.AddItem(menu);

                var entities = await _dynamicEntityService.GetListByDynamicTypeAsync(type.Id);

                foreach (var entity in entities.Where(x=> !x.IsHybrid))
                {
                    menu.AddItem(new ApplicationMenuItem(entity.Name,
                        l[$@"Menu:{entity.Name}"], url: "/dynamicui/" + entity.Id));
                }
            }
        }
    }

    // Nuovo metodo per configurare i menu Hybrid
    private async Task ConfigureHybridMenu(MenuConfigurationContext context)
    {
        AbpAuthorizationService auth = (AbpAuthorizationService)context.AuthorizationService;
        if (!auth.CurrentPrincipal.Identity.IsAuthenticated)
        {
            return;
        }

        var l = context.GetLocalizer<DynamicSpaceResource>();

        var hybridMenu = new ApplicationMenuItem(
            "HybridEntity",
            l["Menu:HybridEntity"],
            icon: "fa fa-cube"
        ).RequireAuthenticated();

        context.Menu.AddItem(hybridMenu);

        var hybridTypes = FindHybridTypes()
            .Where(t => t.Name.EndsWith("Dto"))
            .OrderBy(t => t.Name);


        hybridMenu.AddItem(new ApplicationMenuItem(
               "HybridCompanyDto",
               "HybridCompany",
               url: $"/hybrid/HybridCompanyDto/d"
           ));

        hybridMenu.AddItem(new ApplicationMenuItem(
              "HybridBuildingDto",
              "HybridBuilding",
              url: $"/hybrid/HybridBuildingDto/d"
          ));

        hybridMenu.AddItem(new ApplicationMenuItem(
              "HybridRoomDto",
              "HybridRoom",
              url: $"/hybrid/HybridRoomDto"
          ));

        hybridMenu.AddItem(new ApplicationMenuItem("Totems",
           l["Menu:Totems"], url: "/totems"))
           .RequirePermissions(DynamicSpacePermissions.Totem.Default);

        await Task.CompletedTask;
    }

    // Metodo per trovare tutti i tipi con prefisso "Hybrid" in tutti i namespace di Infocad.DynamicSpace
    private List<Type> FindHybridTypes()
    {
        Assembly contractsAssembly = Assembly.Load("Infocad.DynamicSpace.Application.Contracts");
        return contractsAssembly
            .GetTypes()
            .Where(t => t.Namespace != null &&
                        t.Namespace.StartsWith("Infocad.DynamicSpace") &&
                        t.Name.StartsWith("Hybrid") &&
                        t.IsClass &&
                        !t.IsAbstract)
            .ToList();
    }

    private async Task ConfigureUserMenuAsync(MenuConfigurationContext context)
    {
        if (OperatingSystem.IsBrowser())
        {
            //Blazor wasm menu items

            var authServerUrl = _configuration["AuthServer:Authority"] ?? "";
            var accountResource = context.GetLocalizer<AccountResource>();

            context.Menu.AddItem(new ApplicationMenuItem("Account.Manage", accountResource["MyAccount"], $"{authServerUrl.EnsureEndsWith('/')}Account/Manage", icon: "fa fa-cog", order: 900, target: "_blank").RequireAuthenticated());

        }
        else
        {
            //Blazor server menu items

        }
        await Task.CompletedTask;
    }
}