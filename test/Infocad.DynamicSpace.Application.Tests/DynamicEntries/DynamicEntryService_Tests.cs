using System;
using System.Linq;
using System.Threading.Tasks;
using Infocad.DynamicSpace.DynamicEntities;
using Infocad.DynamicSpace.DynamicEntries;
using Infocad.DynamicSpace.DynamicEntry;
using Infocad.DynamicSpace.DynamicTypes;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Xunit;

namespace Infocad.DynamicSpace.DynamicEntries;

public abstract class DynamicEntryService_Tests<TStartupModule> : DynamicSpaceApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IDynamicEntryService _dynamicEntryService;
    private readonly IDynamicTypeService _dynamicTypeService;
    private readonly IDynamicEntityService _dynamicEntityService;

    protected DynamicEntryService_Tests()
    {
        _dynamicEntryService = GetRequiredService<IDynamicEntryService>();
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
    public async Task Should_Create_A_Valid_Entry()
    {
        // Arrange
      var (dynamicType, dynamicEntity) = await CreateDynamicTypeAndEntityAsync(
            "TestType",
            "TestDescription",
            "TestEntity",
            "TestDescription"
        );
        
        var dtoDynamicEntry = new DynamicEntryDto
        {
            DynamicEntityId = dynamicEntity.Id
        };
        dtoDynamicEntry.SetProperty("Key1", "Value1");
        dtoDynamicEntry.SetProperty("Key2", 123);
        
        // Act
        var result = await _dynamicEntryService.CreateAsync(dtoDynamicEntry);

        // Assert
        result.Id.ShouldNotBe(Guid.Empty);
        result.GetProperty("Key1").ShouldBe("Value1");
        result.GetProperty("Key2").ShouldBe(123);
    }

    [Fact]
    public async Task Should_Get_All_Entries()
    {
        // Arrange
        var (dynamicType, dynamicEntity) = await CreateDynamicTypeAndEntityAsync(
            "TestType",
            "TestDescription",
            "TestEntity",
            "TestDescription"
        );
        var dtoDynamicEntry = new DynamicEntryDto
        {
            DynamicEntityId = dynamicEntity.Id
        };
        dtoDynamicEntry.SetProperty("Key1", "Value1");
        var entry = await _dynamicEntryService.CreateAsync(dtoDynamicEntry);

        // Act
        var result = await _dynamicEntryService.GetListEntryByEntityAsync(dynamicEntity.Id, new GetDynamicEntryListDto()
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
        result.Items.ShouldContain(x => x.Id == entry.Id);
        result.Items.First().GetProperty("Key1").ShouldBe("Value1");
    }

    [Fact]
    public async Task Should_Update_An_Entry()
    {
        // Arrange
        var (dynamicType, dynamicEntity) = await CreateDynamicTypeAndEntityAsync(
            "TestType",
            "TestDescription",
            "TestEntity",
            "TestDescription"
        );

        var dtoDynamicEntry = new DynamicEntryDto
        {
            DynamicEntityId = dynamicEntity.Id
        };
        dtoDynamicEntry.SetProperty("Key1", "Value1");


        // Act
        var entry = await _dynamicEntryService.CreateAsync(dtoDynamicEntry);

        // Act
        entry.SetProperty("Key1", "UpdatedValue");
        var updatedEntry = await _dynamicEntryService.UpdateAsync(entry.Id, entry);

        // Assert
        updatedEntry.ShouldNotBeNull();
        updatedEntry.Id.ShouldBe(entry.Id);
        updatedEntry.GetProperty("Key1").ShouldBe("UpdatedValue");
        updatedEntry.DynamicEntityId.ShouldBe(dynamicEntity.Id);
    }

    [Fact]
    public async Task Should_Delete_An_Entry()
    {
        // Arrange
        var (dynamicType, dynamicEntity) = await CreateDynamicTypeAndEntityAsync(
            "TestType",
            "TestDescription",
            "TestEntity",
            "TestDescription"
        );
        
        var dtoDynamicEntry = new DynamicEntryDto
        {
            DynamicEntityId = dynamicEntity.Id
        };
        dtoDynamicEntry.SetProperty("Key1", "Value1");

        var entry = await _dynamicEntryService.CreateAsync(dtoDynamicEntry);

        // Act
        await _dynamicEntryService.DeleteAsync(entry.Id);

        // Assert
        var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _dynamicEntryService.GetUpdateDynamicEntry(entry.Id);
        });
        exception.Message.ShouldBe("DynamicEntryNotFound");
    }
}