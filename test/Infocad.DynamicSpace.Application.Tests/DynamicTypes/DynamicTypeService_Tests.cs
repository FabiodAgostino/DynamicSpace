using System;
using System.Linq;
using System.Threading.Tasks;
using Infocad.DynamicSpace.DynamicAttirbutes;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Xunit;

namespace Infocad.DynamicSpace.DynamicTypes;

public abstract class DynamicTypeService_Tests<TStartupModule> : DynamicSpaceApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IDynamicTypeService _dynamicTypeService;
    
    protected DynamicTypeService_Tests()
    {
        _dynamicTypeService = GetRequiredService<IDynamicTypeService>();
    }
    
    [Fact]
    public async Task Should_Create_A_Valid_Book()
    {
        //Act
        
        var result = await _dynamicTypeService.CreateAsync(
            new CreateDynamicTypeDto
            {
                Name = "New Type Entity",
                Description = "New Type Entity Description"
            }
        );

        //Assert
        result.Id.ShouldNotBe(Guid.Empty);
        result.Name.ShouldBe("New Type Entity");
    }
    
    [Fact]
    public async Task Should_Get_All_DynamicTypes()
    {
        //Act
        var result = await _dynamicTypeService.GetListAsync(new PagedAndSortedResultRequestDto());
        
        //Assert
        result.TotalCount.ShouldBeGreaterThan(0);
        result.Items.ShouldContain(x => x.Name == "Entity 1");
    }
    
    [Fact]
    public async Task Should_Not_Create_A_DynamicType_Without_Name()
    {
        //Act
        var exception = await Assert.ThrowsAsync<AbpValidationException>(async () =>
        {
            await _dynamicTypeService.CreateAsync(
                new CreateDynamicTypeDto
                {
                    Name = "",
                    Description = "New Type Entity Description"
                }
            );
        });
        //Assert
        exception.ValidationErrors.ShouldContain(err => err.MemberNames.Any(mem => mem == "Name"));
    }
    
    [Fact]
    public async Task Should_Update_An_Existing_DynamicType()
    {
        // Arrange
        var entityType = await _dynamicTypeService.CreateAsync(new CreateDynamicTypeDto
        {
            Name = "Old Name",
            Description = "Old Description"
        });

        // Act
        var result = await _dynamicTypeService.UpdateAsync(entityType.Id, new CreateDynamicTypeDto()
        {
            Name = "Updated Name",
            Description = "Updated Description"
        });

        // Assert
        result.Id.ShouldBe(entityType.Id);
        result.Name.ShouldBe("Updated Name");
        result.Description.ShouldBe("Updated Description");
    }

    [Fact]
    public async Task Should_Delete_An_Existing_DynamicType()
    {
        // Arrange
        var entityType = await _dynamicTypeService.CreateAsync(new CreateDynamicTypeDto
        {
            Name = "Type to Delete",
            Description = "Description to Delete"
        });

        // Act
        await _dynamicTypeService.DeleteAsync(entityType.Id);

        // Assert
        var result = await _dynamicTypeService.GetListAsync(new PagedAndSortedResultRequestDto());
        result.Items.ShouldNotContain(x => x.Id == entityType.Id);
    }
    
}