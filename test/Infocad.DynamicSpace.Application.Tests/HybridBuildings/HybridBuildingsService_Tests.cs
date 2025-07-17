using Infocad.DynamicSpace.DynamicEntities;
using Infocad.DynamicSpace.DynamicTypes;
using Shouldly;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Modularity;
using Xunit;

namespace Infocad.DynamicSpace.HybridBuildings;

public abstract class HybridBuildingService_Tests<TStartupModule> : DynamicSpaceApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IHybridBuildingService _buildingService;
    private readonly IDynamicTypeService _dynamicTypeService;
    private readonly IDynamicEntityService _dynamicEntityService;

    protected HybridBuildingService_Tests()
    {
        _buildingService = GetRequiredService<IHybridBuildingService>();
        _dynamicEntityService = GetRequiredService<IDynamicEntityService>();
        _dynamicTypeService = GetRequiredService<IDynamicTypeService>();
    }

    /// <summary>
    /// Crea un DynamicType e un DynamicEntity associato.
    /// </summary>
    protected async Task<(DynamicTypeDto dynamicType, DynamicEntityDto dynamicEntity)> CreateDynamicTypeAndEntityAsync(string typeName, string typeDescription, string entityName, string entityDescription)
    {
        var dynamicType = await _dynamicTypeService.CreateAsync(new CreateDynamicTypeDto
        {
            Name = typeName,
            Description = typeDescription,
        });

        var dynamicEntity = await _dynamicEntityService.CreateAsync(new CreateDynamicEntityDto
        {
            Name = entityName,
            Description = entityDescription,
            DynamicTypeId = dynamicType.Id
        });

        return (dynamicType, dynamicEntity);
    }

    [Fact]
    public async Task Should_Create_A_Valid_Building()
    {
        // Arrange
        var (dynamicType, dynamicEntity) = await CreateDynamicTypeAndEntityAsync(
            "EdificioType",
            "Tipo per edifici",
            "StruttureEdifici",
            "Entità per strutture edifici"
        );

        var dtoBuilding = new HybridBuildingDto
        {
            DynamicEntityId = dynamicEntity.Id,
            Name = "Edificio Test",
            X = 45.4642f,
            Y = 9.1900f
        };

        dtoBuilding.SetProperty("Altezza", "120");
        dtoBuilding.SetProperty("AnnoCostruzione", "2010");
        dtoBuilding.SetProperty("NumeriPiani", "15");

        // Act
        var result = await _buildingService.CreateAsync(dtoBuilding);

        // Assert
        result.Id.ShouldNotBe(Guid.Empty);
        result.Name.ShouldBe("Edificio Test");
        result.X.ShouldBe(45.4642f);
        result.Y.ShouldBe(9.1900f);
        result.GetProperty("Altezza").ShouldBe("120");
        result.GetProperty("AnnoCostruzione").ShouldBe("2010");
        result.GetProperty("NumeriPiani").ShouldBe("15");
    }

    [Fact]
    public async Task Should_Get_All_Buildings()
    {
        // Arrange
        var (dynamicType, dynamicEntity) = await CreateDynamicTypeAndEntityAsync(
            "EdificioType",
            "Tipo per edifici",
            "StruttureEdifici",
            "Entità per strutture edifici"
        );

        var dtoBuilding = new HybridBuildingDto
        {
            DynamicEntityId = dynamicEntity.Id,
            Name = "Grattacielo Alfa",
            X = 45.4642f,
            Y = 9.1900f
        };

        dtoBuilding.SetProperty("Altezza", "150");

        var building = await _buildingService.CreateAsync(dtoBuilding);

        // Act
        var result = await _buildingService.GetListObjects(dynamicEntity.Id, new GetHybridBuildingListDto()
        {
            MaxResultCount = 10,
            SkipCount = 0,
            Sorting = "Id",
            Filter = null
        }
        );

        // Assert
        result.TotalCount.ShouldBeGreaterThan(0);
        result.Items.ShouldNotBeEmpty();
        result.Items.ShouldContain(b => b.Name == "Grattacielo Alfa");
    }

    [Fact]
    public async Task Should_Get_Filtered_Buildings()
    {
        // Arrange
        var (dynamicType, dynamicEntity) = await CreateDynamicTypeAndEntityAsync(
            "EdificioType",
            "Tipo per edifici",
            "StruttureEdifici",
            "Entità per strutture edifici"
        );

        // Creiamo diversi edifici per testare il filtro
        var building1 = new HybridBuildingDto
        {
            DynamicEntityId = dynamicEntity.Id,
            Name = "Torre Nord",
            X = 45.4642f,
            Y = 9.1900f
        };
        await _buildingService.CreateAsync(building1);

        var building2 = new HybridBuildingDto
        {
            DynamicEntityId = dynamicEntity.Id,
            Name = "Palazzo Est",
            X = 41.9028f,
            Y = 12.4964f
        };
        await _buildingService.CreateAsync(building2);

        var building3 = new HybridBuildingDto
        {
            DynamicEntityId = dynamicEntity.Id,
            Name = "Torre Sud",
            X = 45.4642f,
            Y = 9.1900f
        };
        await _buildingService.CreateAsync(building3);

        // Act - Otteniamo tutti gli edifici ordinati per nome
        var result = await _buildingService.GetListObjects(dynamicEntity.Id, new GetHybridBuildingListDto()
        {
            MaxResultCount = 10,
            SkipCount = 0,
            Sorting = "Name",
        }
        );

        // Assert
        result.Items.ShouldContain(b => b.Name == "Palazzo Est");
        result.Items.ShouldContain(b => b.Name == "Torre Nord");
        result.Items.ShouldContain(b => b.Name == "Torre Sud");
        Should.Throw<Shouldly.ShouldAssertException>(() => {
            result.Items.ShouldNotContain(b => b.Name == "Palazzo Est");
        });
    }

    [Fact]
    public async Task Should_Update_A_Building()
    {
        // Arrange
        var (dynamicType, dynamicEntity) = await CreateDynamicTypeAndEntityAsync(
            "EdificioType",
            "Tipo per edifici",
            "StruttureEdifici",
            "Entità per strutture edifici"
        );

        var dtoBuilding = new HybridBuildingDto
        {
            DynamicEntityId = dynamicEntity.Id,
            Name = "Centro Direzionale",
            X = 41.9028f,
            Y = 12.4964f
        };

        dtoBuilding.SetProperty("Altezza", "80");
        dtoBuilding.SetProperty("NumeriPiani", "12");

        var building = await _buildingService.CreateAsync(dtoBuilding);

        // Act - Aggiorniamo i dati dell'edificio
        building.Name = "Centro Direzionale Beta";
        building.X = 41.9030f;
        building.SetProperty("Altezza", "85");
        building.SetProperty("NumeroUffici", "120");  // Aggiungiamo una nuova proprietà

        var updatedBuilding = await _buildingService.UpdateAsync(building.Id, building);

        // Assert
        updatedBuilding.Name.ShouldBe("Centro Direzionale Beta");
        updatedBuilding.X.ShouldBe(41.9030f);
        updatedBuilding.GetProperty("Altezza").ShouldBe("85");
        updatedBuilding.GetProperty("NumeroUffici").ShouldBe("120");
        updatedBuilding.GetProperty("NumeriPiani").ShouldBe("12");  // Questa non è cambiata
    }

    [Fact]
    public async Task Should_Delete_A_Building()
    {
        // Arrange
        var (dynamicType, dynamicEntity) = await CreateDynamicTypeAndEntityAsync(
            "EdificioType",
            "Tipo per edifici",
            "StruttureEdifici",
            "Entità per strutture edifici"
        );

        var dtoBuilding = new HybridBuildingDto
        {
            DynamicEntityId = dynamicEntity.Id,
            Name = "Edificio Temporaneo",
            X = 43.7696f,
            Y = 11.2558f
        };

        var building = await _buildingService.CreateAsync(dtoBuilding);

        // Act
        await _buildingService.DeleteAsync(building.Id);

        // Assert
        var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _buildingService.GetUpdateObject(building.Id);
        });
        exception.Message.ShouldBe("BuildingNotFound");
    }

    [Fact]
    public async Task Should_Get_Building_For_Update()
    {
        // Arrange
        var (dynamicType, dynamicEntity) = await CreateDynamicTypeAndEntityAsync(
            "EdificioType",
            "Tipo per edifici",
            "StruttureEdifici",
            "Entità per strutture edifici"
        );

        var dtoBuilding = new HybridBuildingDto
        {
            DynamicEntityId = dynamicEntity.Id,
            Name = "Palazzo Delta",
            X = 45.0703f,
            Y = 7.6869f
        };

        dtoBuilding.SetProperty("Altezza", "65");
        dtoBuilding.SetProperty("AnnoRistrutturazione", "2018");

        var building = await _buildingService.CreateAsync(dtoBuilding);

        // Act
        var retrievedBuilding = await _buildingService.GetUpdateObject(building.Id);

        // Assert
        retrievedBuilding.ShouldNotBeNull();
        retrievedBuilding.Id.ShouldBe(building.Id);
        retrievedBuilding.Name.ShouldBe("Palazzo Delta");
        retrievedBuilding.X.ShouldBe(45.0703f);
        retrievedBuilding.Y.ShouldBe(7.6869f);
        retrievedBuilding.GetProperty("Altezza").ShouldBe("65");
        retrievedBuilding.GetProperty("AnnoRistrutturazione").ShouldBe("2018");
    }

    [Fact]
    public async Task Should_Throw_Exception_When_Building_Not_Found()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _buildingService.GetUpdateObject(Guid.NewGuid());
        });

        exception.Message.ShouldBe("BuildingNotFound");
    }
}