using System;
using System.Linq;
using System.Threading.Tasks;
using Infocad.DynamicSpace.DynamicEntities;
using Volo.Abp.Modularity;
using Xunit;

namespace Infocad.DynamicSpace.DynamicHierarchies;

public abstract class DynamicHIerarchyService_Tests<TStartupModule> : DynamicSpaceApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private IDynamicHierarchyService _dynamicHierarchyService;
    private IDynamicHierarchyRepository _dynamicHierarchyRepository;
    private IDynamicEntityRepository _dynamicEntityRepository;
    
    protected DynamicHIerarchyService_Tests()
    {
        _dynamicHierarchyService = GetRequiredService<IDynamicHierarchyService>();
        _dynamicHierarchyRepository = GetRequiredService<IDynamicHierarchyRepository>();
        _dynamicEntityRepository = GetRequiredService<IDynamicEntityRepository>();
    }
    
    [Fact]
    public async Task Should_Create_DynamicHierarchy()
    {
        // Arrange
        var input = new CreateDynamicHierarchyDto
        {
            Name = "Test Hierarchy",
            Description = "This is a test hierarchy",
            Default = true
        };
    
        // Act
        var result = await _dynamicHierarchyService.CreateAsync(input);
    
        // Assert
        Assert.NotNull(result);
        // Altri assert specifici
    }
    
    [Fact]
    public async Task Should_Get_DynamicHierarchy_By_Id()
    {
        // Arrange
        var input = new CreateDynamicHierarchyDto
        {
            Name = "Test Hierarchy for get by Id",
            Description = "Test Hierarchy for get by Id",
            Default = true
        };
        var dynamicHierarchy = await _dynamicHierarchyService.CreateAsync(input);
    
        // Act
        var result = await _dynamicHierarchyService.GetAsync(dynamicHierarchy.Id);
    
        // Assert
        Assert.NotNull(result);
        Assert.Equal(dynamicHierarchy.Id, result.Id);
    }
    
    [Fact]
    public async Task Should_Update_DynamicHierarchy()
    {
        // Arrange
        var input = new CreateDynamicHierarchyDto
        {
            Name = "Test Hierarchy for Update by Id",
            Description = "Test Hierarchy for get by Id",
            Default = true
        };
        var dynamicHierarchy = await _dynamicHierarchyService.CreateAsync(input);

        var updateDto = new CreateDynamicHierarchyDto
        {
            Name = "Updated Hierarchy",
            Description = "This is an updated hierarchy",
            Default = false
        };

    
        // Act
        var result = await _dynamicHierarchyService.UpdateAsync(dynamicHierarchy.Id, updateDto);
    
        // Assert
        Assert.NotNull(result);
        // Altri assert specifici
        Assert.Equal("Updated Hierarchy", result.Name);
    }

    [Fact]
    public async Task Should_Delete_DynamicHierarchy()
    {
        // Arrange
        var input = new CreateDynamicHierarchyDto
        {
            Name = "Test Hierarchy for Update by Id",
            Description = "Test Hierarchy for get by Id",
            Default = true
        };
        var dynamicHierarchy = await _dynamicHierarchyService.CreateAsync(input);
        
        // Act
        await _dynamicHierarchyService.DeleteAsync(dynamicHierarchy.Id);
    
        // Assert
        var deleted = await _dynamicHierarchyRepository.FindAsync(dynamicHierarchy.Id);
        Assert.Null(deleted);
    }
    
    [Fact]
        public async Task Should_Create_DynamicHierarchyEntity()
        {
            // Arrange
            var hierarchyInput = new CreateDynamicHierarchyDto
            {
                Name = "Entity Hierarchy",
                Description = "Hierarchy for entity test",
                Default = false
            };
            var hierarchy = await _dynamicHierarchyService.CreateAsync(hierarchyInput);
            var entity1 = await _dynamicEntityRepository.FindByNameAsync("Entity 1");
            var entity2 = await _dynamicEntityRepository.FindByNameAsync("Entity 2");
    
            var entityInput = new CreateDynamicHiererchyEntityDto()
            {
                DynamicSourceEntityId = entity1.Id,
                DynamicTargetEntityId = entity2.Id,
                DisplayFields = "Field1,Field2"
            };
    
            // Act
            var entityResult = await _dynamicHierarchyService.CreateDynamicHiererchyEntityAsync(
                hierarchy.Id,
                entityInput);
    
            // Assert
            Assert.NotNull(entityResult);
            Assert.Equal("Entity Hierarchy", entityResult.Name);
            Assert.NotNull(entityResult.Entities.First());
        }
    
        [Fact]
        public async Task Should_Update_DynamicHierarchyEntity()
        {
            // Arrange
            var hierarchyInput = new CreateDynamicHierarchyDto
            {
                Name = "Entity Hierarchy",
                Description = "Hierarchy for entity test",
                Default = false
            };
            var hierarchy = await _dynamicHierarchyService.CreateAsync(hierarchyInput);
            var entity1 = await _dynamicEntityRepository.FindByNameAsync("Entity 1");
            var entity2 = await _dynamicEntityRepository.FindByNameAsync("Entity 2");
            
            var entityInput = new CreateDynamicHiererchyEntityDto()
            {
                DynamicSourceEntityId = entity1.Id,
                DynamicTargetEntityId = entity2.Id,
                DisplayFields = "Field1,Field2"
            };
    
            var entityResult = await _dynamicHierarchyService.CreateDynamicHiererchyEntityAsync(
                hierarchy.Id,
                entityInput);
        
            
            var updateInput = new UpdateDynamicHierarchyEntityDto()
            {
                Id = entityResult.Entities.FirstOrDefault().Id,
                DynamicSourceEntityId = entity1.Id,
                DynamicTargetEntityId = entity2.Id,
                DisplayFields = "Field3,Field4"
            };
        
            // Act
            var updatedEntity = await _dynamicHierarchyService.UpdateDynamicHiererchyEntityAsync(entityResult.Id, updateInput);
        
            // Assert
            Assert.NotNull(updatedEntity);
            Assert.Equal("Field3,Field4", updatedEntity.Entities.ToList<DynamicHierarchyEntityDto>().FirstOrDefault()?.DisplayFields);
        }
        
        [Fact]
        public async Task Should_Delete_DynamicHierarchyEntity()
        {
            // Arrange
            var hierarchyInput = new CreateDynamicHierarchyDto
            {
                Name = "Entity Hierarchy",
                Description = "Hierarchy for entity test",
                Default = false
            };
            var hierarchy = await _dynamicHierarchyService.CreateAsync(hierarchyInput);
            var entity1 = await _dynamicEntityRepository.FindByNameAsync("Entity 1");
            var entity2 = await _dynamicEntityRepository.FindByNameAsync("Entity 2");
    
            var entityInput = new CreateDynamicHiererchyEntityDto()
            {
                DynamicSourceEntityId = entity1.Id,
                DynamicTargetEntityId = entity2.Id,
                DisplayFields = "Field1,Field2"
            };
            
            var entityResult = await _dynamicHierarchyService.CreateDynamicHiererchyEntityAsync(
                hierarchy.Id,
                entityInput);
            
            var entityToDelete = entityResult.Entities.FirstOrDefault().Id;
            // Act
            await _dynamicHierarchyService.DeleteDynamicHierarchyEntityAsync(hierarchy.Id, entityResult.Entities.FirstOrDefault().Id);
        
            // Assert
            var deletedEntity = await _dynamicHierarchyRepository.GetByIdIncludeEntitiesAsync(entityResult.Id);
            Assert.Null(deletedEntity.Entities.FirstOrDefault(e => e.Id == entityToDelete));
        }
}