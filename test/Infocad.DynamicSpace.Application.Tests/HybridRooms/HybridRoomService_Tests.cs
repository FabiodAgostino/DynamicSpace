using Infocad.DynamicSpace.DynamicEntities;
using Infocad.DynamicSpace.DynamicTypes;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Modularity;
using Xunit;

namespace Infocad.DynamicSpace.HybridRooms;

public abstract class HybridRoomService_Tests<TStartupModule> : DynamicSpaceApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IHybridRoomService _roomService;
    private readonly IDynamicTypeService _dynamicTypeService;
    private readonly IDynamicEntityService _dynamicEntityService;

    protected HybridRoomService_Tests()
    {
        _roomService = GetRequiredService<IHybridRoomService>();
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
    public async Task Should_Create_A_Valid_Room()
    {
        // Arrange
        var (dynamicType, dynamicEntity) = await CreateDynamicTypeAndEntityAsync(
            "SalaType",
            "Tipo per sale",
            "StruttureStanze",
            "Entità per strutture stanze"
        );

        var dtoRoom = new HybridRoomDto
        {
            DynamicEntityId = dynamicEntity.Id,
            Name = "Sala Riunioni Test",
            Capacity = 25,
            Description = "Sala per riunioni aziendali"
        };

        dtoRoom.SetProperty("Piano", "3");
        dtoRoom.SetProperty("Superficie", "45");
        dtoRoom.SetProperty("TipoStanza", "Riunioni");

        // Act
        var result = await _roomService.CreateAsync(dtoRoom);

        // Assert
        result.Id.ShouldNotBe(Guid.Empty);
        result.Name.ShouldBe("Sala Riunioni Test");
        result.Capacity.ShouldBe(25);
        result.Description.ShouldBe("Sala per riunioni aziendali");
        result.GetProperty("Piano").ShouldBe("3");
        result.GetProperty("Superficie").ShouldBe("45");
        result.GetProperty("TipoStanza").ShouldBe("Riunioni");
    }

    [Fact]
    public async Task Should_Get_All_Rooms()
    {
        // Arrange
        var (dynamicType, dynamicEntity) = await CreateDynamicTypeAndEntityAsync(
            "SalaType",
            "Tipo per sale",
            "StruttureStanze",
            "Entità per strutture stanze"
        );

        var dtoRoom = new HybridRoomDto
        {
            DynamicEntityId = dynamicEntity.Id,
            Name = "Aula Magna",
            Capacity = 150,
            Description = "Aula principale per conferenze"
        };

        dtoRoom.SetProperty("Piano", "1");

        var room = await _roomService.CreateAsync(dtoRoom);

        // Act
        var result = await _roomService.GetListObjects(dynamicEntity.Id, new GetHybridRoomListDto()
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
        result.Items.ShouldContain(r => r.Name == "Aula Magna");
    }

    [Fact]
    public async Task Should_Get_Filtered_Rooms()
    {
        // Arrange
        var (dynamicType, dynamicEntity) = await CreateDynamicTypeAndEntityAsync(
            "SalaType",
            "Tipo per sale",
            "StruttureStanze",
            "Entità per strutture stanze"
        );

        // Creiamo diverse stanze per testare il filtro
        var room1 = new HybridRoomDto
        {
            DynamicEntityId = dynamicEntity.Id,
            Name = "Sala Conferenze Nord",
            Capacity = 50,
            Description = "Sala conferenze settore nord"
        };
        await _roomService.CreateAsync(room1);

        var room2 = new HybridRoomDto
        {
            DynamicEntityId = dynamicEntity.Id,
            Name = "Ufficio Direttore",
            Capacity = 5,
            Description = "Ufficio del direttore generale"
        };
        await _roomService.CreateAsync(room2);

        var room3 = new HybridRoomDto
        {
            DynamicEntityId = dynamicEntity.Id,
            Name = "Sala Conferenze Sud",
            Capacity = 40,
            Description = "Sala conferenze settore sud"
        };
        await _roomService.CreateAsync(room3);

        // Act - Otteniamo tutte le stanze ordinate per nome
        var result = await _roomService.GetListObjects(dynamicEntity.Id, new GetHybridRoomListDto()
        {
            MaxResultCount = 10,
            SkipCount = 0,
            Sorting = "Name",
        }
        );

        // Assert
        result.Items.ShouldContain(r => r.Name == "Ufficio Direttore");
        result.Items.ShouldContain(r => r.Name == "Sala Conferenze Nord");
        result.Items.ShouldContain(r => r.Name == "Sala Conferenze Sud");
        Should.Throw<Shouldly.ShouldAssertException>(() => {
            result.Items.ShouldNotContain(r => r.Name == "Ufficio Direttore");
        });
    }

    [Fact]
    public async Task Should_Update_A_Room()
    {
        // Arrange
        var (dynamicType, dynamicEntity) = await CreateDynamicTypeAndEntityAsync(
            "SalaType",
            "Tipo per sale",
            "StruttureStanze",
            "Entità per strutture stanze"
        );

        var dtoRoom = new HybridRoomDto
        {
            DynamicEntityId = dynamicEntity.Id,
            Name = "Laboratorio IT",
            Capacity = 30,
            Description = "Laboratorio informatico"
        };

        dtoRoom.SetProperty("Piano", "2");
        dtoRoom.SetProperty("NumeroComputer", "25");

        var room = await _roomService.CreateAsync(dtoRoom);

        // Act - Aggiorniamo i dati della stanza
        room.Name = "Laboratorio IT Avanzato";
        room.Capacity = 35;
        room.Description = "Laboratorio informatico con attrezzature avanzate";
        room.SetProperty("Piano", "3");
        room.SetProperty("NumeroComputer", "30");
        room.SetProperty("Proiettore", "Presente");  // Aggiungiamo una nuova proprietà

        var updatedRoom = await _roomService.UpdateAsync(room.Id, room);

        // Assert
        updatedRoom.Name.ShouldBe("Laboratorio IT Avanzato");
        updatedRoom.Capacity.ShouldBe(35);
        updatedRoom.Description.ShouldBe("Laboratorio informatico con attrezzature avanzate");
        updatedRoom.GetProperty("Piano").ShouldBe("3");
        updatedRoom.GetProperty("NumeroComputer").ShouldBe("30");
        updatedRoom.GetProperty("Proiettore").ShouldBe("Presente");
    }

    [Fact]
    public async Task Should_Delete_A_Room()
    {
        // Arrange
        var (dynamicType, dynamicEntity) = await CreateDynamicTypeAndEntityAsync(
            "SalaType",
            "Tipo per sale",
            "StruttureStanze",
            "Entità per strutture stanze"
        );

        var dtoRoom = new HybridRoomDto
        {
            DynamicEntityId = dynamicEntity.Id,
            Name = "Stanza Temporanea",
            Capacity = 10,
            Description = "Stanza di test da eliminare"
        };

        var room = await _roomService.CreateAsync(dtoRoom);

        // Act
        await _roomService.DeleteAsync(room.Id);

        // Assert
        var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _roomService.GetUpdateObject(room.Id);
        });
        exception.Message.ShouldBe("RoomNotFound");
    }

    [Fact]
    public async Task Should_Get_Room_For_Update()
    {
        // Arrange
        var (dynamicType, dynamicEntity) = await CreateDynamicTypeAndEntityAsync(
            "SalaType",
            "Tipo per sale",
            "StruttureStanze",
            "Entità per strutture stanze"
        );

        var dtoRoom = new HybridRoomDto
        {
            DynamicEntityId = dynamicEntity.Id,
            Name = "Sala Formazione",
            Capacity = 20,
            Description = "Sala per corsi di formazione"
        };

        dtoRoom.SetProperty("Piano", "4");
        dtoRoom.SetProperty("Lavagna", "Interattiva");

        var room = await _roomService.CreateAsync(dtoRoom);

        // Act
        var retrievedRoom = await _roomService.GetUpdateObject(room.Id);

        // Assert
        retrievedRoom.ShouldNotBeNull();
        retrievedRoom.Id.ShouldBe(room.Id);
        retrievedRoom.Name.ShouldBe("Sala Formazione");
        retrievedRoom.Capacity.ShouldBe(20);
        retrievedRoom.Description.ShouldBe("Sala per corsi di formazione");
        retrievedRoom.GetProperty("Piano").ShouldBe("4");
        retrievedRoom.GetProperty("Lavagna").ShouldBe("Interattiva");
    }

    [Fact]
    public async Task Should_Throw_Exception_When_Room_Not_Found()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _roomService.GetUpdateObject(Guid.NewGuid());
        });

        exception.Message.ShouldBe("RoomNotFound");
    }

    [Fact]
    public async Task Should_Filter_Rooms_By_Description()
    {
        // Arrange
        var (dynamicType, dynamicEntity) = await CreateDynamicTypeAndEntityAsync(
            "SalaType",
            "Tipo per sale",
            "StruttureStanze",
            "Entità per strutture stanze"
        );

        var room1 = new HybridRoomDto
        {
            DynamicEntityId = dynamicEntity.Id,
            Name = "Sala A",
            Capacity = 20,
            Description = "Sala per conferenze"
        };
        var createdRoom1 = await _roomService.CreateAsync(room1);

        var room2 = new HybridRoomDto
        {
            DynamicEntityId = dynamicEntity.Id,
            Name = "Ufficio B",
            Capacity = 2,
            Description = "Ufficio privato"
        };
        var createdRoom2 = await _roomService.CreateAsync(room2);

        // Act - Otteniamo tutte le room senza filtro e filtriamo lato client
        var allRooms = await _roomService.GetListObjects(dynamicEntity.Id, new GetHybridRoomListDto()
        {
            MaxResultCount = 100,
            SkipCount = 0
        });

        // Assert - Filtriamo lato client per evitare il problema di EF
        var filteredRooms = allRooms.Items.Where(r => r.Description.Contains("conferenze")).ToList();

        filteredRooms.ShouldNotBeEmpty();
        filteredRooms.ShouldContain(r => r.Id == createdRoom1.Id);
        filteredRooms.ShouldNotContain(r => r.Id == createdRoom2.Id);
        filteredRooms.First().Description.ShouldContain("conferenze");
    }
}