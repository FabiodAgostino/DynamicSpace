using Infocad.DynamicSpace.DynamicAttributes;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Xunit;

namespace Infocad.DynamicSpace.DynamicRules
{
    public abstract class DynamicRuleService_Tests<TStartupModule> : DynamicSpaceApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IDynamicRuleService _dynamicRuleService;

        protected DynamicRuleService_Tests()
        {
            _dynamicRuleService = GetRequiredService<IDynamicRuleService>();
        }

        [Fact]
        public async Task Should_Create_A_Valid_Regex_Rule()
        {
            // Act
            var result = await _dynamicRuleService.CreateAsync(
                new CreateDynamicRuleDto
                {
                    Name = "Email Validation",
                    Description = "Validazione formato email",
                    AttributeType = DynamicAttributeType.Text,
                    Rule = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"
                }
            );

            // Assert
            result.Id.ShouldNotBe(Guid.Empty);
            result.Name.ShouldBe("Email Validation");
            result.Description.ShouldBe("Validazione formato email");
            result.Rule.ShouldBe(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
        }

        [Fact]
        public async Task Should_Get_All_DynamicRules()
        {
            // Act
            var result = await _dynamicRuleService.GetListAsync(new PagedAndSortedResultRequestDto());

            // Assert
            result.TotalCount.ShouldBeGreaterThan(0);
            result.Items.ShouldContain(x => x.Name == "Alphanumeric Only");
        }

        [Fact]
        public async Task Should_Not_Create_A_DynamicRule_Without_Name()
        {
            // Act
            var exception = await Assert.ThrowsAsync<AbpValidationException>(async () =>
            {
                await _dynamicRuleService.CreateAsync(
                    new CreateDynamicRuleDto
                    {
                        Name = "",
                        Description = "Descrizione regola",
                        Rule = @"^[A-Za-z0-9]+$"
                    }
                );
            });

            // Assert
            exception.ValidationErrors.ShouldContain(err => err.MemberNames.Any(mem => mem == "Name"));
        }

        [Fact]
        public async Task Should_Not_Create_A_DynamicRule_Without_Rule()
        {
            // Act
            var exception = await Assert.ThrowsAsync<AbpValidationException>(async () =>
            {
                await _dynamicRuleService.CreateAsync(
                    new CreateDynamicRuleDto
                    {
                        Name = "Test Rule",
                        Description = "Descrizione regola",
                        AttributeType = DynamicAttributeType.Text,
                        Rule = ""
                    }
                );
            });

            // Assert
            exception.ValidationErrors.ShouldContain(err => err.MemberNames.Any(mem => mem == "Rule"));
        }

        [Fact]
        public async Task Should_Update_An_Existing_DynamicRule()
        {
            // Arrange
            var rule = await _dynamicRuleService.CreateAsync(new CreateDynamicRuleDto
            {
                Name = "Codice Fiscale",
                Description = "Validazione Codice Fiscale",
                Rule = @"^[A-Z]{6}[0-9]{2}[A-Z][0-9]{2}[A-Z][0-9]{3}[A-Z]$",
                AttributeType = DynamicAttributeType.Text
            });

            // Act
            var result = await _dynamicRuleService.UpdateAsync(rule.Id, new UpdateDynamicRuleDto
            {
                Name = "Partita IVA",
                Description = "Validazione Partita IVA",
                Rule = @"^[0-9]{11}$",
                AttributeType = DynamicAttributeType.Number
            });

            // Assert
            result.Id.ShouldBe(rule.Id);
            result.Name.ShouldBe("Partita IVA");
            result.Description.ShouldBe("Validazione Partita IVA");
            result.Rule.ShouldBe(@"^[0-9]{11}$");
        }

        [Fact]
        public async Task Should_Delete_An_Existing_DynamicRule()
        {
            // Arrange
            var rule = await _dynamicRuleService.CreateAsync(new CreateDynamicRuleDto
            {
                Name = "Telefono IT",
                Description = "Validazione numero di telefono italiano",
                Rule = @"^\+?39?[0-9]{10}$",
                AttributeType = DynamicAttributeType.Text
            });

            // Act
            await _dynamicRuleService.DeleteAsync(rule.Id);

            // Assert
            var result = await _dynamicRuleService.GetListAsync(new PagedAndSortedResultRequestDto());
            result.Items.ShouldNotContain(x => x.Id == rule.Id);
        }

        [Fact]
        public async Task Should_Support_Different_AttributeTypes()
        {
            // Act
            var textRule = await _dynamicRuleService.CreateAsync(
                new CreateDynamicRuleDto
                {
                    Name = "Testo Test",
                    Description = "Regola per testo",
                    Rule = @"^[A-Za-z]+$",
                    AttributeType = DynamicAttributeType.Text
                }
            );

            var numberRule = await _dynamicRuleService.CreateAsync(
                new CreateDynamicRuleDto
                {
                    Name = "Numero Test",
                    Description = "Regola per numeri",
                    Rule = @"^[0-9]+$",
                    AttributeType = DynamicAttributeType.Number
                }
            );

            var dateRule = await _dynamicRuleService.CreateAsync(
                new CreateDynamicRuleDto
                {
                    Name = "Data Test",
                    Description = "Regola per date",
                    Rule = @"^\d{4}-\d{2}-\d{2}$",
                    AttributeType = DynamicAttributeType.DateTime
                }
            );

            // Assert
            textRule.AttributeType.ShouldBe(DynamicAttributeType.Text);
            numberRule.AttributeType.ShouldBe(DynamicAttributeType.Number);
            dateRule.AttributeType.ShouldBe(DynamicAttributeType.DateTime);
        }

        [Fact]
        public async Task Should_Get_Rule_By_Id()
        {
            // Arrange
            var createdRule = await _dynamicRuleService.CreateAsync(new CreateDynamicRuleDto
            {
                Name = "Solo Consonanti",
                Description = "Validazione testo con solo consonanti",
                Rule = @"^[bcdfghjklmnpqrstvwxyzBCDFGHJKLMNPQRSTVWXYZ]+$",
                AttributeType = DynamicAttributeType.Text
            });

            // Act
            var result = await _dynamicRuleService.GetAsync(createdRule.Id);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createdRule.Id);
            result.Name.ShouldBe(createdRule.Name);
        }

        [Fact]
        public async Task Should_Create_Rule_Without_Description()
        {
            // Act
            var result = await _dynamicRuleService.CreateAsync(
                new CreateDynamicRuleDto
                {
                    Name = "Targa IT",
                    Rule = @"^[A-Z]{2}[0-9]{3}[A-Z]{2}$",
                    AttributeType = DynamicAttributeType.Text
                }
            );

            // Assert
            result.Id.ShouldNotBe(Guid.Empty);
            result.Name.ShouldBe("Targa IT");
            result.Description.ShouldBeNull();
        }

        [Fact]
        public async Task Should_Validate_Phone_Number_Regex()
        {
            // Act
            var result = await _dynamicRuleService.CreateAsync(
                new CreateDynamicRuleDto
                {
                    Name = "Phone Validation",
                    Description = "Validazione numero di telefono",
                    Rule = @"^(\+\d{1,3}[- ]?)?\d{10}$",
                    AttributeType = DynamicAttributeType.Text
                }
            );

            // Assert
            result.Id.ShouldNotBe(Guid.Empty);
            result.Rule.ShouldBe(@"^(\+\d{1,3}[- ]?)?\d{10}$");
        }

        [Fact]
        public async Task Should_Validate_Password_Strength_Regex()
        {
            // Act
            var result = await _dynamicRuleService.CreateAsync(
                new CreateDynamicRuleDto
                {
                    Name = "Strong Password",
                    Description = "Password forte: minimo 8 caratteri, una maiuscola, una minuscola, un numero e un carattere speciale",
                    Rule = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
                    AttributeType = DynamicAttributeType.Text
                }
            );

            // Assert
            result.Id.ShouldNotBe(Guid.Empty);
            result.Rule.ShouldBe(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
        }

        [Fact]
        public async Task Should_Validate_Date_Format_Regex()
        {
            // Act
            var result = await _dynamicRuleService.CreateAsync(
                new CreateDynamicRuleDto
                {
                    Name = "Date DD/MM/YYYY",
                    Description = "Validazione formato data DD/MM/YYYY",
                    Rule = @"^(0[1-9]|[12][0-9]|3[01])/(0[1-9]|1[0-2])/\d{4}$",
                    AttributeType = DynamicAttributeType.DateTime
                }
            );

            // Assert
            result.Id.ShouldNotBe(Guid.Empty);
            result.Rule.ShouldBe(@"^(0[1-9]|[12][0-9]|3[01])/(0[1-9]|1[0-2])/\d{4}$");
        }
    }
}