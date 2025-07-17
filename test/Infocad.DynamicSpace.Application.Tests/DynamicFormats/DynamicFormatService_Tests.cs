using System;
using System.Linq;
using System.Threading.Tasks;
using Infocad.DynamicSpace.DynamicAttributes;
using Infocad.DynamicSpace.DynamicFormats;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Xunit;

namespace Infocad.DynamicSpace.DynamicFormats
{
    public abstract class DynamicFormatService_Tests<TStartupModule> : DynamicSpaceApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IDynamicFormatService _dynamicFormatService;

        protected DynamicFormatService_Tests()
        {
            _dynamicFormatService = GetRequiredService<IDynamicFormatService>();
        }

        [Fact]
        public async Task Should_Create_A_Valid_Format()
        {
            // Act
            var result = await _dynamicFormatService.CreateAsync(
                new CreateDynamicFormatDto
                {
                    Name = "Test Format",
                    Description = "Format per test",
                    AttributeType = DynamicAttributeType.Text,
                    FormatPattern = "^[A-Z]*$"
                }
            );

            // Assert
            result.Id.ShouldNotBe(Guid.Empty);
            result.Name.ShouldBe("Test Format");
            result.AttributeType.ShouldBe(DynamicAttributeType.Text);
            result.FormatPattern.ShouldBe("^[A-Z]*$");
        }

        [Fact]
        public async Task Should_Get_All_DynamicFormats()
        {
            // Act
            var result = await _dynamicFormatService.GetListAsync(new PagedAndSortedResultRequestDto());

            // Assert
            result.TotalCount.ShouldBeGreaterThan(0);
            result.Items.ShouldContain(x => x.AttributeType == DynamicAttributeType.Text);
        }

        [Fact]
        public async Task Should_Not_Create_A_DynamicFormat_Without_Name()
        {
            // Act
            var exception = await Assert.ThrowsAsync<AbpValidationException>(async () =>
            {
                await _dynamicFormatService.CreateAsync(
                    new CreateDynamicFormatDto
                    {
                        Name = "",
                        Description = "Descrizione formato",
                        AttributeType = DynamicAttributeType.Number,
                        FormatPattern = "0.00"
                    }
                );
            });

            // Assert
            exception.ValidationErrors.ShouldContain(err => err.MemberNames.Any(mem => mem == "Name"));
        }

        //[Fact]
        //public async Task Should_Update_An_Existing_DynamicFormat()
        //{
        //    // Arrange
        //    var format = await _dynamicFormatService.CreateAsync(new CreateDynamicFormatDto
        //    {
        //        Name = "Format da aggiornare",
        //        Description = "Vecchia descrizione",
        //        AttributeType = DynamicAttributeType.DateTime,
        //        FormatPattern = "dd/MM/yyyy"
        //    });

        //    // Act
        //    var result = await _dynamicFormatService.UpdateAsync(format.Id, new UpdateDynamicFormatDto
        //    {
        //        Name = "Format aggiornato",
        //        Description = "Nuova descrizione",
        //        AttributeType = DynamicAttributeType.DateTime,
        //        FormatPattern = "yyyy-MM-dd"
        //    });

        //    // Assert
        //    result.Id.ShouldBe(format.Id);
        //    result.Name.ShouldBe("Format aggiornato");
        //    result.Description.ShouldBe("Nuova descrizione");
        //    result.FormatPattern.ShouldBe("yyyy-MM-dd");
        //}

        [Fact]
        public async Task Should_Delete_An_Existing_DynamicFormat()
        {
            // Arrange
            var format = await _dynamicFormatService.CreateAsync(new CreateDynamicFormatDto
            {
                Name = "Format da eliminare",
                Description = "Descrizione da eliminare",
                AttributeType = DynamicAttributeType.Boolean,
                FormatPattern = "Si|No"
            });

            // Act
            await _dynamicFormatService.DeleteAsync(format.Id);

            // Assert
            var result = await _dynamicFormatService.GetListAsync(new PagedAndSortedResultRequestDto());
            result.Items.ShouldNotContain(x => x.Id == format.Id);
        }

        [Fact]
        public async Task Should_Get_Format_By_Id()
        {
            // Arrange
            var createdFormat = await _dynamicFormatService.CreateAsync(new CreateDynamicFormatDto
            {
                Name = "Format per GetById",
                Description = "Descrizione test GetById",
                AttributeType = DynamicAttributeType.Text,
                FormatPattern = "^[a-zA-Z]*$"
            });

            // Act
            var result = await _dynamicFormatService.GetAsync(createdFormat.Id);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createdFormat.Id);
            result.Name.ShouldBe(createdFormat.Name);
        }


        [Fact]
        public async Task Should_Create_Format_Without_Description()
        {
            // Act
            var result = await _dynamicFormatService.CreateAsync(
                new CreateDynamicFormatDto
                {
                    Name = "Format senza descrizione",
                    AttributeType = DynamicAttributeType.Number,
                    FormatPattern = "0.0"
                }
            );

            // Assert
            result.Id.ShouldNotBe(Guid.Empty);
            result.Name.ShouldBe("Format senza descrizione");
            result.Description.ShouldBeNull();
        }

        [Fact]
        public async Task Should_Create_Format_Without_FormatPattern()
        {
            // Act
            var result = await _dynamicFormatService.CreateAsync(
                new CreateDynamicFormatDto
                {
                    Name = "Format senza pattern",
                    Description = "Descrizione format senza pattern",
                    AttributeType = DynamicAttributeType.Text
                }
            );

            // Assert
            result.Id.ShouldNotBe(Guid.Empty);
            result.Name.ShouldBe("Format senza pattern");
            result.FormatPattern.ShouldBeNull();
        }
    }
}