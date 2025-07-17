using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infocad.DynamicSpace.DynamicTypes;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Xunit;

namespace Infocad.DynamicSpace.DynamicAttributes;

public abstract class DynamicAttributeService_Tests<TStartupModule> : DynamicSpaceApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IDynamicAttributeService _dynamicTypeAttributeService;

    protected DynamicAttributeService_Tests()
    {
        _dynamicTypeAttributeService = GetRequiredService<IDynamicAttributeService>();
    }

    [Fact]
    public async Task Should_Create_A_Valid_Attribute()
    {
        //Act
        var result = await _dynamicTypeAttributeService.CreateAsync(
            new CreateDynamicAttributeDto
            {
                Name = "New Attribute Entity",
                Description = "New Attribute Entity Description",
                Type = DynamicAttributeType.Number,
            }
        );

        //Assert
        result.Id.ShouldNotBe(Guid.Empty);
        result.Name.ShouldBe("New Attribute Entity");
    }

    [Fact]
    public async Task Should_Create_A_Valid_Two_Attribute()
    {
        //Act
        var result = await _dynamicTypeAttributeService.CreateAsync(
            new CreateDynamicAttributeDto
            {
                Name = "New Attribute Entity",
                Description = "New Attribute Entity Description",
                Type = DynamicAttributeType.Number,
            }
        );

        var result2 = await _dynamicTypeAttributeService.CreateAsync(
            new CreateDynamicAttributeDto
            {
                Name = "New Attribute Entity2",
                Description = "New Attribute Entity Description2",
                Type = DynamicAttributeType.Text,
            }
        );

        //Assert
        result.Id.ShouldNotBe(Guid.Empty);
        result.Name.ShouldBe("New Attribute Entity");

        result2.Id.ShouldNotBe(Guid.Empty);
        result2.Name.ShouldBe("New Attribute Entity2");
    }


    [Fact]
    public async Task Should_Get_All_DynamicAttributes()
    {
        // Act
        var result = await _dynamicTypeAttributeService.GetListAsync(new PagedAndSortedResultRequestDto());

        // Assert
        result.TotalCount.ShouldBeGreaterThan(0);
        result.Items.ShouldContain(x => x.Name == "Attribute 1");
    }


    [Fact]
    public async Task Should_Not_Create_A_DynamicAttribute_Without_Name()
    {
        // Act
        var exception = await Assert.ThrowsAsync<AbpValidationException>(async () =>
        {
            await _dynamicTypeAttributeService.CreateAsync(
                new CreateDynamicAttributeDto
                {
                    Name = "",
                    Description = "Invalid Attribute",
                    Type = DynamicAttributeType.Text
                }
            );
        });

        // Assert
        exception.ValidationErrors.ShouldContain(err => err.MemberNames.ToList().Any(mem => mem == "Name"));
    }

    [Fact]
    public async Task Should_Update_An_Existing_DynamicAttribute()
    {
        // Arrange
        var existingAttribute = await _dynamicTypeAttributeService.CreateAsync(
            new CreateDynamicAttributeDto
            {
                Name = "Existing Attribute",
                Description = "Existing Attribute Description",
                Type = DynamicAttributeType.Text
            }
        );

        // Act
        var updatedAttribute = await _dynamicTypeAttributeService.UpdateAsync(
            existingAttribute.Id,
            new CreateDynamicAttributeDto()
            {
                Name = "Updated Attribute",
                Description = "Updated Attribute Description",
                Type = DynamicAttributeType.Text
            }
        );

        // Assert
        updatedAttribute.Id.ShouldBe(existingAttribute.Id);
        updatedAttribute.Name.ShouldBe("Updated Attribute");
        updatedAttribute.Description.ShouldBe("Updated Attribute Description");
    }

    [Fact]
    public async Task Should_Delete_An_Existing_DynamicAttribute()
    {
        // Arrange
        var existingAttribute = await _dynamicTypeAttributeService.CreateAsync(
            new CreateDynamicAttributeDto
            {
                Name = "Attribute to Delete",
                Description = "Attribute Description",
                Type = DynamicAttributeType.Text
            }
        );

        // Act
        await _dynamicTypeAttributeService.DeleteAsync(existingAttribute.Id);

        // Assert
        var allAttributes = await _dynamicTypeAttributeService.GetListAsync(new PagedAndSortedResultRequestDto());
        allAttributes.Items.ShouldNotContain(x => x.Id == existingAttribute.Id);
    }

    [Fact]
    public async Task Should_Get_DynamicAttributes_By_Guids()
    {
        // Arrange
        var attribute1 = await _dynamicTypeAttributeService.CreateAsync(
            new CreateDynamicAttributeDto
            {
                Name = "Attribute 1",
                Description = "Description 1",
                Type = DynamicAttributeType.Text
            }
        );

        var attribute2 = await _dynamicTypeAttributeService.CreateAsync(
            new CreateDynamicAttributeDto
            {
                Name = "Attribute 2",
                Description = "Description 2",
                Type = DynamicAttributeType.Number
            }
        );

        var guids = new List<Guid> { attribute1.Id, attribute2.Id };

        // Act
        var result = await _dynamicTypeAttributeService.GetListByGuids(guids);

        // Assert
        result.Count.ShouldBe(2);
        result.ShouldContain(x => x.Id == attribute1.Id && x.Name == "Attribute 1");
        result.ShouldContain(x => x.Id == attribute2.Id && x.Name == "Attribute 2");
    }

    [Fact]
    public async Task Should_Create_DynamicAttribute_With_Navigation_Type()
    {
        // Act
        var result = await _dynamicTypeAttributeService.CreateAsync(
            new CreateDynamicAttributeDto
            {
                Name = "Navigation Attribute",
                Description = "Attribute of type Navigation",
                Type = DynamicAttributeType.Navigation,
                NavigationSettings =
                    "{\"EntityType\":\"Infocad.DynamicSpace.DynamicEntities.DynamicEntity\",\"DisplayFields\":[\"Name\",\"Description\"]}"
            }
        );

        // Assert
        result.Id.ShouldNotBe(Guid.Empty);
        result.Name.ShouldBe("Navigation Attribute");
        result.Type.ShouldBe(DynamicAttributeType.Navigation);
    }

    [Fact]
    public async Task Fail_Create_DynamicAttribute_With_Navigation_Type()
    {
        var exception = await Assert.ThrowsAsync<AbpValidationException>(async () =>
        {
            await _dynamicTypeAttributeService.CreateAsync(
                new CreateDynamicAttributeDto
                {
                    Name = "Navigation Attribute",
                    Description = "Invalid Attribute",
                    Type = DynamicAttributeType.Navigation
                }
            );
        });

        // Assert
        exception.ValidationErrors.ShouldContain(err => err.ErrorMessage.Contains("NavigationSettings"));
    }

    [Fact]
    public async Task Should_Update_DynamicAttribute_With_Navigation_Type()
    {
        // Arrange
        var existingAttribute = await _dynamicTypeAttributeService.CreateAsync(
            new CreateDynamicAttributeDto
            {
                Name = "Navigation Attribute",
                Description = "Attribute of type Navigation",
                Type = DynamicAttributeType.Navigation,
                NavigationSettings =
                    "{\"EntityType\":\"Infocad.DynamicSpace.DynamicEntities.DynamicEntity\",\"DisplayFields\":[\"Name\",\"Description\"]}"
            }
        );

        // Act
        var updatedAttribute = await _dynamicTypeAttributeService.UpdateAsync(
            existingAttribute.Id,
            new CreateDynamicAttributeDto
            {
                Name = "Updated Navigation Attribute",
                Description = "Updated Attribute of type Navigation",
                Type = DynamicAttributeType.Navigation,
                NavigationSettings =
                    "{\"EntityType\":\"Infocad.DynamicSpace.DynamicEntities.DynamicEntity\",\"DisplayFields\":[\"Name\"]}"
            }
        );

        // Assert
        updatedAttribute.Id.ShouldBe(existingAttribute.Id);
        updatedAttribute.Name.ShouldBe("Updated Navigation Attribute");
        updatedAttribute.Type.ShouldBe(DynamicAttributeType.Navigation);
        updatedAttribute.NavigationSettings.ShouldContain("DisplayFields");
    }

    [Fact]
    public async Task Fail_Update_DynamicAttribute_With_Navigation_Type_Without_Settings()
    {
        // Arrange
        var existingAttribute = await _dynamicTypeAttributeService.CreateAsync(
            new CreateDynamicAttributeDto
            {
                Name = "Navigation Attribute",
                Description = "Attribute of type Navigation",
                Type = DynamicAttributeType.Navigation,
                NavigationSettings =
                    "{\"EntityType\":\"Infocad.DynamicSpace.DynamicEntities.DynamicEntity\",\"DisplayFields\":[\"Name\",\"Description\"]}"
            }
        );

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AbpValidationException>(async () =>
        {
            await _dynamicTypeAttributeService.UpdateAsync(
                existingAttribute.Id,
                new CreateDynamicAttributeDto
                {
                    Name = "Invalid Navigation Attribute",
                    Description = "Invalid update",
                    Type = DynamicAttributeType.Navigation
                    // NavigationSettings mancante
                }
            );
        });

        exception.ValidationErrors.ShouldContain(err => err.ErrorMessage.Contains("NavigationSettings"));
    }
    
    [Fact]
    public async Task Should_Get_Navigation_Child_Entities()
    {
        // Arrange
        var navigationAttribute = await _dynamicTypeAttributeService.CreateAsync(
            new CreateDynamicAttributeDto
            {
                Name = "Navigation Attribute",
                Description = "Attribute with navigation children",
                Type = DynamicAttributeType.Navigation,
                NavigationSettings =
                    "{\"Entity\":\"d82e4925-9662-9926-7066-3a1ab8943344\",\"IsHybrid\":true,\"NavField\":\"ID\",\"UIFiled\":\"Name\",\"EntityName\":\"Edifici\"}"
            }
        );

        // Act
        var childEntities = await _dynamicTypeAttributeService.GetNavChildEntities(new Guid("d82e4925-9662-9926-7066-3a1ab8943344"));

        // Assert
        childEntities.ShouldNotBeNull();
        childEntities.Count.ShouldBeGreaterThan(0);
    }
}