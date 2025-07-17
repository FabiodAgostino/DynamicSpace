using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Xunit;

namespace Infocad.DynamicSpace.Totems
{
    public abstract class TotemService_Tests<TStartupModule> : DynamicSpaceApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly ITotemService _totemService;

        protected TotemService_Tests()
        {
            _totemService = GetRequiredService<ITotemService>();
        }

        [Fact]
        public async Task Should_Create_A_Valid_Totem()
        {
            // Act
            var result = await _totemService.CreateAsync(
                new CreateTotemDto
                {
                    Name = "Totem Principale",
                    Description = "Totem informativo principale",
                    X = 100,
                    Y = 200
                }
            );

            // Assert
            result.Id.ShouldNotBe(Guid.Empty);
            result.Name.ShouldBe("Totem Principale");
            result.Description.ShouldBe("Totem informativo principale");
            result.X.ShouldBe(100);
            result.Y.ShouldBe(200);
        }

        [Fact]
        public async Task Should_Get_All_Totems()
        {
            // Act
            var result = await _totemService.GetListAsync(new PagedAndSortedResultRequestDto());

            // Assert
            result.TotalCount.ShouldBeGreaterThan(0);
            result.Items.ShouldContain(x => x.Name == "Totem Ingresso");
        }


        [Fact]
        public async Task Should_Create_Totem_With_Zero_Coordinates()
        {
            // Act
            var result = await _totemService.CreateAsync(
                new CreateTotemDto
                {
                    Name = "Totem Origine",
                    Description = "Totem posizionato all'origine",
                    X = 0,
                    Y = 0
                }
            );

            // Assert
            result.X.ShouldBe(0);
            result.Y.ShouldBe(0);
        }

        [Fact]
        public async Task Should_Create_Totem_With_Negative_Coordinates()
        {
            // Act
            var result = await _totemService.CreateAsync(
                new CreateTotemDto
                {
                    Name = "Totem Negativo",
                    Description = "Totem con coordinate negative",
                    X = -50,
                    Y = -100
                }
            );

            // Assert
            result.X.ShouldBe(-50);
            result.Y.ShouldBe(-100);
        }

        [Fact]
        public async Task Should_Update_An_Existing_Totem()
        {
            // Arrange
            var totem = await _totemService.CreateAsync(new CreateTotemDto
            {
                Name = "Nome Originale",
                Description = "Descrizione Originale",
                X = 10,
                Y = 20
            });

            // Act
            var result = await _totemService.UpdateAsync(totem.Id, new CreateTotemDto
            {
                Name = "Nome Aggiornato",
                Description = "Descrizione Aggiornata",
                X = 30,
                Y = 40
            });

            // Assert
            result.Id.ShouldBe(totem.Id);
            result.Name.ShouldBe("Nome Aggiornato");
            result.Description.ShouldBe("Descrizione Aggiornata");
            result.X.ShouldBe(30);
            result.Y.ShouldBe(40);
        }

        [Fact]
        public async Task Should_Delete_An_Existing_Totem()
        {
            // Arrange
            var totem = await _totemService.CreateAsync(new CreateTotemDto
            {
                Name = "Totem da Eliminare",
                Description = "Descrizione da Eliminare",
                X = 999,
                Y = 999
            });

            // Act
            await _totemService.DeleteAsync(totem.Id);

            // Assert
            var result = await _totemService.GetListAsync(new PagedAndSortedResultRequestDto());
            result.Items.ShouldNotContain(x => x.Id == totem.Id);
        }

        [Fact]
        public async Task Should_Get_Totem_By_Id()
        {
            // Arrange
            var createdTotem = await _totemService.CreateAsync(new CreateTotemDto
            {
                Name = "Totem da Recuperare",
                Description = "Descrizione totem da recuperare",
                X = 150,
                Y = 250
            });

            // Act
            var result = await _totemService.GetAsync(createdTotem.Id);

            // Assert
            result.Id.ShouldBe(createdTotem.Id);
            result.Name.ShouldBe("Totem da Recuperare");
            result.Description.ShouldBe("Descrizione totem da recuperare");
            result.X.ShouldBe(150);
            result.Y.ShouldBe(250);
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Getting_NonExistent_Totem()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act & Assert
            await Should.ThrowAsync<EntityNotFoundException>(async () =>
            {
                await _totemService.GetAsync(nonExistentId);
            });
        }

        [Fact]
        public async Task Should_Update_Only_Position_Of_Existing_Totem()
        {
            // Arrange - Verifica che possiamo aggiornare solo le coordinate mantenendo nome e descrizione
            var originalTotem = await _totemService.CreateAsync(new CreateTotemDto
            {
                Name = "Totem Fisso",
                Description = "Descrizione che non cambia",
                X = 100,
                Y = 100
            });

            // Act
            var result = await _totemService.UpdateAsync(originalTotem.Id, new CreateTotemDto
            {
                Name = "Totem Fisso",
                Description = "Descrizione che non cambia",
                X = 200,
                Y = 300
            });

            // Assert
            result.Id.ShouldBe(originalTotem.Id);
            result.Name.ShouldBe("Totem Fisso");
            result.Description.ShouldBe("Descrizione che non cambia");
            result.X.ShouldBe(200);
            result.Y.ShouldBe(300);
        }
    }
}