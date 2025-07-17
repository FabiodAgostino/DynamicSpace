using Infocad.DynamicSpace.DynamicAttributes;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Xunit;

namespace Infocad.DynamicSpace.DynamicControls;

public abstract class DynamicControlService_Tests<TStartupModule> : DynamicSpaceApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IDynamicControlService _dynamicControlService;

    protected DynamicControlService_Tests()
    {
        _dynamicControlService = GetRequiredService<IDynamicControlService>();
    }

    [Fact]
    public async Task Should_Create_A_Valid_DynamicControl()
    {
        // Act
        var result = await _dynamicControlService.CreateAsync(
            new CreateDynamicControlDto
            {
                Name = "New Control",
                Description = "New Control Description",
                Type = DynamicAttributeType.Text,
                ComponentType = "TextBox"
            }
        );

        // Assert
        result.Id.ShouldNotBe(Guid.Empty);
        result.Name.ShouldBe("New Control");
        result.Description.ShouldBe("New Control Description");
        result.Type.ShouldBe(DynamicAttributeType.Text);
        result.ComponentType.ShouldBe("TextBox");
    }

    [Fact]
    public async Task Should_Get_All_DynamicControls()
    {
        // Arrange - Crea prima alcuni controlli
        await _dynamicControlService.CreateAsync(new CreateDynamicControlDto
        {
            Name = "Test Control 1",
            Description = "Test Description 1",
            Type = DynamicAttributeType.Text,
            ComponentType = "TextBox"
        });

        await _dynamicControlService.CreateAsync(new CreateDynamicControlDto
        {
            Name = "Test Control 2",
            Description = "Test Description 2",
            Type = DynamicAttributeType.Number,
            ComponentType = "NumericInput"
        });

        // Act
        var result = await _dynamicControlService.GetListAsync(new PagedAndSortedResultRequestDto());

        // Assert
        result.TotalCount.ShouldBeGreaterThanOrEqualTo(2);
        result.Items.ShouldNotBeNull();
        result.Items.Count.ShouldBeGreaterThanOrEqualTo(2);
        result.Items.ShouldContain(x => x.Name == "Test Control 1");
        result.Items.ShouldContain(x => x.Name == "Test Control 2");
    }

    [Fact]
    public async Task Should_Not_Create_A_DynamicControl_Without_Name()
    {
        // Act & Assert
        var exception = await Should.ThrowAsync<AbpValidationException>(async () =>
        {
            await _dynamicControlService.CreateAsync(
                new CreateDynamicControlDto
                {
                    Name = "", // Nome vuoto
                    Description = "New Control Description",
                    Type = DynamicAttributeType.Text,
                    ComponentType = "TextBox"
                }
            );
        });

        // Assert
        exception.ValidationErrors.ShouldContain(err => err.MemberNames.Any(mem => mem == "Name"));
    }

    [Fact]
    public async Task Should_Not_Create_A_DynamicControl_Without_Name_Null()
    {
        // Act & Assert
        var exception = await Should.ThrowAsync<AbpValidationException>(async () =>
        {
            await _dynamicControlService.CreateAsync(
                new CreateDynamicControlDto
                {
                    Name = null, // Nome null
                    Description = "New Control Description",
                    Type = DynamicAttributeType.Text,
                    ComponentType = "TextBox"
                }
            );
        });

        // Assert
        exception.ValidationErrors.ShouldContain(err => err.MemberNames.Any(mem => mem == "Name"));
    }

    [Fact]
    public async Task Should_Not_Create_A_DynamicControl_Without_ComponentType()
    {
        // Act & Assert
        var exception = await Should.ThrowAsync<AbpValidationException>(async () =>
        {
            await _dynamicControlService.CreateAsync(
                new CreateDynamicControlDto
                {
                    Name = "New Control",
                    Description = "New Control Description",
                    Type = DynamicAttributeType.Text,
                    ComponentType = "" // ComponentType vuoto
                }
            );
        });

        // Assert
        exception.ValidationErrors.ShouldContain(err => err.MemberNames.Any(mem => mem == "ComponentType"));
    }

    [Fact]
    public async Task Should_Not_Create_A_DynamicControl_Without_ComponentType_Null()
    {
        // Act & Assert
        var exception = await Should.ThrowAsync<AbpValidationException>(async () =>
        {
            await _dynamicControlService.CreateAsync(
                new CreateDynamicControlDto
                {
                    Name = "New Control",
                    Description = "New Control Description",
                    Type = DynamicAttributeType.Text,
                    ComponentType = null // ComponentType null
                }
            );
        });

        // Assert
        exception.ValidationErrors.ShouldContain(err => err.MemberNames.Any(mem => mem == "ComponentType"));
    }

    [Fact]
    public async Task Should_Update_An_Existing_DynamicControl()
    {
        // Arrange
        var dynamicControl = await _dynamicControlService.CreateAsync(new CreateDynamicControlDto
        {
            Name = "Old Control Name",
            Description = "Old Description",
            Type = DynamicAttributeType.Text,
            ComponentType = "TextBox"
        });

        // Act
        var result = await _dynamicControlService.UpdateAsync(dynamicControl.Id, new UpdateDynamicControlDto
        {
            Id = dynamicControl.Id,
            Name = "Updated Control Name",
            Description = "Updated Description",
            Type = DynamicAttributeType.Number,
            ComponentType = "NumericInput"
        });

        // Assert
        result.Id.ShouldBe(dynamicControl.Id);
        result.Name.ShouldBe("Updated Control Name");
        result.Description.ShouldBe("Updated Description");
        result.Type.ShouldBe(DynamicAttributeType.Number);
        result.ComponentType.ShouldBe("NumericInput");
    }

    [Fact]
    public async Task Should_Delete_An_Existing_DynamicControl()
    {
        // Arrange
        var dynamicControl = await _dynamicControlService.CreateAsync(new CreateDynamicControlDto
        {
            Name = "Control to Delete",
            Description = "Description to Delete",
            Type = DynamicAttributeType.Boolean,
            ComponentType = "CheckBox"
        });

        // Act
        await _dynamicControlService.DeleteAsync(dynamicControl.Id);

        // Assert
        var result = await _dynamicControlService.GetListAsync(new PagedAndSortedResultRequestDto());
        result.Items.ShouldNotContain(x => x.Id == dynamicControl.Id);
    }

    [Fact]
    public async Task Should_Get_DynamicControl_By_Id()
    {
        // Arrange
        var dynamicControl = await _dynamicControlService.CreateAsync(new CreateDynamicControlDto
        {
            Name = "Control to Get",
            Description = "Description to Get",
            Type = DynamicAttributeType.DateTime,
            ComponentType = "DatePicker"
        });

        // Act
        var result = await _dynamicControlService.GetAsync(dynamicControl.Id);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(dynamicControl.Id);
        result.Name.ShouldBe("Control to Get");
        result.Description.ShouldBe("Description to Get");
        result.Type.ShouldBe(DynamicAttributeType.DateTime);
        result.ComponentType.ShouldBe("DatePicker");
    }

    [Fact]
    public async Task Should_Create_DynamicControls_With_Different_Types()
    {
        // Arrange & Act
        var textControl = await _dynamicControlService.CreateAsync(new CreateDynamicControlDto
        {
            Name = "Text Control",
            Type = DynamicAttributeType.Text,
            ComponentType = "TextBox"
        });

        var numberControl = await _dynamicControlService.CreateAsync(new CreateDynamicControlDto
        {
            Name = "Number Control",
            Type = DynamicAttributeType.Number,
            ComponentType = "NumericInput"
        });

        var booleanControl = await _dynamicControlService.CreateAsync(new CreateDynamicControlDto
        {
            Name = "Boolean Control",
            Type = DynamicAttributeType.Boolean,
            ComponentType = "CheckBox"
        });

        // Assert
        textControl.Type.ShouldBe(DynamicAttributeType.Text);
        numberControl.Type.ShouldBe(DynamicAttributeType.Number);
        booleanControl.Type.ShouldBe(DynamicAttributeType.Boolean);
    }
}