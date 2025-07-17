using AutoMapper.Internal.Mappers;
using Infocad.DynamicSpace.DynamicAttributes;
using Infocad.DynamicSpace.DynamicEntityAttributes;
using Infocad.DynamicSpace.DynamicTypes;
using Shouldly;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Xunit;

namespace Infocad.DynamicSpace.DynamicEntities
{
    public abstract class DynamicEntityService_Test<TStartupModule> : DynamicSpaceApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {
        private readonly IDynamicEntityService _dynamicEntityService;
        private readonly IDynamicAttributeService _dynamicAttributeService;
        private readonly IDynamicEntityRepository _dynamicEntityRepository;
        private readonly IDynamicTypeService _dynamicTypeService;

        protected DynamicEntityService_Test()
        {
            _dynamicEntityService = GetRequiredService<IDynamicEntityService>();
            _dynamicAttributeService = GetRequiredService<IDynamicAttributeService>();
            _dynamicEntityRepository = GetRequiredService<IDynamicEntityRepository>();
            _dynamicTypeService = GetRequiredService<IDynamicTypeService>();

        }


        [Fact]
        public async Task GetListAsync_Restituisce_PagedResult_Correttamente()
        {
            //Act
            var result = await _dynamicEntityService.GetListAsync(new GetDynamicEntityListDto());

            //Assert
            result.TotalCount.ShouldBeGreaterThan(0);
            result.Items.ShouldContain(x => x.Name == "Entity 1");
        }

        [Fact]
        public async Task Should_Create_A_Valid_Entity()
        {
            var attributes = await _dynamicAttributeService.GetListAsync(new PagedAndSortedResultRequestDto { MaxResultCount = 1 });
            var guidAttribute = attributes.Items.FirstOrDefault().Id;

            var types = await _dynamicTypeService.GetListAsync(new PagedAndSortedResultRequestDto { MaxResultCount = 1 });
            var guidTypes = types.Items.FirstOrDefault().Id;

            var entity = new CreateDynamicEntityDto
            {
                Name = "Test",
                Description = "Test",
                DynamicTypeId = guidTypes
            };
            entity.Attributes.Add(new CreateDynamicEntityAttributeDto() { Label = "Attributo1", Order = 0, DynamicAttributeId = guidAttribute });

            var result = await _dynamicEntityService.CreateAsync(entity);

            result.ShouldNotBeNull();
            result.Name.ShouldBe("Test");
            result.Description.ShouldBe("Test");
            result.DynamicTypeId.ShouldBe(guidTypes);
        }

        [Fact]
        public async Task Should_Create_A_Valid_Entity_Without_Description()
        {
            var attributes = await _dynamicAttributeService.GetListAsync(new PagedAndSortedResultRequestDto { MaxResultCount = 1 });
            var guidAttribute = attributes.Items.FirstOrDefault().Id;

            var types = await _dynamicTypeService.GetListAsync(new PagedAndSortedResultRequestDto { MaxResultCount = 1 });
            var guidTypes = types.Items.FirstOrDefault().Id;

            var entity = new CreateDynamicEntityDto
            {
                Name = "Test",
                DynamicTypeId = guidTypes
            };
            entity.Attributes.Add(new CreateDynamicEntityAttributeDto() { Label = "Attributo1", Order = 0, DynamicAttributeId = guidAttribute });

            var result = await _dynamicEntityService.CreateAsync(entity);

            result.ShouldNotBeNull();
            result.Name.ShouldBe("Test");
            result.DynamicTypeId.ShouldBe(guidTypes);
        }

        [Fact]
        public async Task Should_Create_A_Valid_Entity_Without_Format_And_Set_Order()
        {
            var attributes = await _dynamicAttributeService.GetListAsync(new PagedAndSortedResultRequestDto { MaxResultCount = 1 });
            var guidAttribute = attributes.Items.FirstOrDefault().Id;

            var types = await _dynamicTypeService.GetListAsync(new PagedAndSortedResultRequestDto { MaxResultCount = 1 });
            var guidTypes = types.Items.FirstOrDefault().Id;

            var entity = new CreateDynamicEntityDto
            {
                Name = "Test",
                DynamicTypeId = guidTypes
            };
            entity.Attributes.Add(new CreateDynamicEntityAttributeDto() { Label = "Attributo1", Order = 1, DynamicAttributeId = guidAttribute });

            var result = await _dynamicEntityService.CreateAsync(entity);

            result.ShouldNotBeNull();
            result.Name.ShouldBe("Test");
            result.DynamicTypeId.ShouldBe(guidTypes);
            result.Attributes.ShouldContain(x => x.Order == 1);
        }

        [Fact]
        public async Task Should_Create_A_Valid_Entity_With_Multiple_Attribute()
        {
            // Arrange
            var attributes = await _dynamicAttributeService.GetListAsync(new PagedAndSortedResultRequestDto { MaxResultCount = 2 });
            var guidAttribute1 = attributes.Items.FirstOrDefault().Id;
            var guidAttribute2 = attributes.Items.LastOrDefault().Id;


            var types = await _dynamicTypeService.GetListAsync(new PagedAndSortedResultRequestDto { MaxResultCount = 1 });
            var guidTypes = types.Items.FirstOrDefault().Id;

            var entity = new CreateDynamicEntityDto
            {
                Name = "Test",
                Description = "Test",
                DynamicTypeId = guidTypes
            };
            entity.Attributes.Add(new CreateDynamicEntityAttributeDto() { Label = "Attributo1", Order = 0, DynamicAttributeId = guidAttribute1 });
            entity.Attributes.Add(new CreateDynamicEntityAttributeDto() { Label = "Attributo2", Order = 1, DynamicAttributeId = guidAttribute2 });

            var result = await _dynamicEntityService.CreateAsync(entity);

            result.ShouldNotBeNull();
            result.Name.ShouldBe("Test");
            result.Description.ShouldBe("Test");
            result.DynamicTypeId.ShouldBe(guidTypes);
            result.Attributes.Count.ShouldBe(2);
        }

        [Fact]
        public async Task Should_Delete_Entity()
        {
            var attributes = await _dynamicAttributeService.GetListAsync(new PagedAndSortedResultRequestDto { MaxResultCount = 1 });
            var guidAttribute = attributes.Items.FirstOrDefault().Id;

            var types = await _dynamicTypeService.GetListAsync(new PagedAndSortedResultRequestDto { MaxResultCount = 1 });
            var guidTypes = types.Items.FirstOrDefault().Id;

            var entity = new CreateDynamicEntityDto
            {
                Name = "EntityToDelete",
                Description = "Test entity for deletion",
                DynamicTypeId = guidTypes
            };
            entity.Attributes.Add(new CreateDynamicEntityAttributeDto() { Label = "Attributo1", Order = 0, DynamicAttributeId = guidAttribute });

            var createdEntity = await _dynamicEntityService.CreateAsync(entity);

            // Act
            await _dynamicEntityService.DeleteAsync(createdEntity.Id);

            // Assert
            var entities = await _dynamicEntityService.GetListAsync(new GetDynamicEntityListDto());
            entities.Items.ShouldNotContain(x => x.Id == createdEntity.Id);
        }

        [Fact]
        public async Task Should_Delete_Entity_With_Attribute()
        {
            var entities = await _dynamicEntityService.GetListAsync(new GetDynamicEntityListDto() { MaxResultCount = 1 });
            var entity = entities.Items.FirstOrDefault();
            var attribute = entity.Attributes.FirstOrDefault();
            await _dynamicEntityService.DeleteAttributeAsync(entity.Id, attribute.DynamicAttributeId);
        }


        [Fact]
        public async Task Should_Not_Create_A_DynamicEntity_Without_Name()
        {
            var types = await _dynamicTypeService.GetListAsync(new PagedAndSortedResultRequestDto { MaxResultCount = 1 });
            var guidTypes = types.Items.FirstOrDefault().Id;
            // Act
            var exception = await Assert.ThrowsAsync<AbpValidationException>(async () =>
            {
                await _dynamicEntityService.CreateAsync(
                    new CreateDynamicEntityDto
                    {
                        DynamicTypeId = guidTypes,
                        Description = "Test",
                        Name = string.Empty,
                    }
                );
            });

            // Assert
            exception.ValidationErrors.ShouldContain(err => err.MemberNames.ToList().Any(mem => mem == "Name"));


        }
    }
}
