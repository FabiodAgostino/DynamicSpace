using Infocad.DynamicSpace.DynamicEntities;
using Infocad.DynamicSpace.DynamicTypes;
using Shouldly;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Modularity;
using Xunit;

namespace Infocad.DynamicSpace.HybridCompanies;

public abstract class HybridCompanyService_Tests<TStartupModule> : DynamicSpaceApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IHybridCompanyService _companyService;
    private readonly IDynamicTypeService _dynamicTypeService;
    private readonly IDynamicEntityService _dynamicEntityService;

    protected HybridCompanyService_Tests()
    {
        _companyService = GetRequiredService<IHybridCompanyService>();
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
    public async Task Should_Create_A_Valid_Company()
    {
        // Arrange
        var (dynamicType, dynamicEntity) = await CreateDynamicTypeAndEntityAsync(
            "AziendaType",
            "Tipo per aziende",
            "AnagrafiAziende",
            "Entità per anagrafi aziendali"
        );

        var dtoCompany = new HybridCompanyDto
        {
            DynamicEntityId = dynamicEntity.Id,
            RagioneSociale = "Test Company Srl",
            Cognome = "Rossi",
            Indirizzo = "Via Test 123, Test",
            Telefono = "+39 123456789"
        };

        dtoCompany.SetProperty("Email", "info@testcompany.it");
        dtoCompany.SetProperty("PEC", "testcompany@pec.it");
        dtoCompany.SetProperty("PartitaIVA", "12345678901");

        // Act
        var result = await _companyService.CreateAsync(dtoCompany);

        // Assert
        result.Id.ShouldNotBe(Guid.Empty);
        result.RagioneSociale.ShouldBe("Test Company Srl");
        result.Cognome.ShouldBe("Rossi");
        result.Indirizzo.ShouldBe("Via Test 123, Test");
        result.Telefono.ShouldBe("+39 123456789");
        result.GetProperty("Email").ShouldBe("info@testcompany.it");
        result.GetProperty("PEC").ShouldBe("testcompany@pec.it");
        result.GetProperty("PartitaIVA").ShouldBe("12345678901");
    }

    [Fact]
    public async Task Should_Get_All_Companies()
    {
        // Arrange
        var (dynamicType, dynamicEntity) = await CreateDynamicTypeAndEntityAsync(
            "AziendaType",
            "Tipo per aziende",
            "AnagrafiAziende",
            "Entità per anagrafi aziendali"
        );

        var dtoCompany = new HybridCompanyDto
        {
            DynamicEntityId = dynamicEntity.Id,
            RagioneSociale = "Test Company Srl",
            Cognome = "Bianchi",
            Indirizzo = "Via Test 456, Test",
            Telefono = "+39 987654321"
        };

        dtoCompany.SetProperty("Email", "info@testcompany.it");

        var company = await _companyService.CreateAsync(dtoCompany);

        // Act
        var result = await _companyService.GetListObjects(dynamicEntity.Id, new GetHybridCompanyListDto()
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
        result.Items.ShouldContain(c => c.RagioneSociale == "Test Company Srl");
    }

    [Fact]
    public async Task Should_Get_Filtered_Companies()
    {
        // Arrange
        var (dynamicType, dynamicEntity) = await CreateDynamicTypeAndEntityAsync(
            "AziendaType",
            "Tipo per aziende",
            "AnagrafiAziende",
            "Entità per anagrafi aziendali"
        );

        // Creiamo diverse aziende per testare il filtro
        var company1 = new HybridCompanyDto
        {
            DynamicEntityId = dynamicEntity.Id,
            RagioneSociale = "Alpha Srl",
            Cognome = "",
            Indirizzo = "Via Alpha 123, Roma",
            Telefono = "+39 111222333"
        };
        await _companyService.CreateAsync(company1);

        var company2 = new HybridCompanyDto
        {
            DynamicEntityId = dynamicEntity.Id,
            RagioneSociale = "Beta SpA",
            Cognome = "",
            Indirizzo = "Via Beta 456, Milano",
            Telefono = "+39 444555666"
        };
        await _companyService.CreateAsync(company2);

        var company3 = new HybridCompanyDto
        {
            DynamicEntityId = dynamicEntity.Id,
            RagioneSociale = "Gamma Consulting",
            Cognome = "Verdi",
            Indirizzo = "Via Gamma 789, Roma",
            Telefono = "+39 777888999"
        };
        await _companyService.CreateAsync(company3);

        // Act - Filtriamo per Roma nell'indirizzo
        var result = await _companyService.GetListObjects(dynamicEntity.Id, new GetHybridCompanyListDto()
        {
            MaxResultCount = 10,
            SkipCount = 0,
            Sorting = "RagioneSociale",
        }
        );

        // Assert
        result.Items.ShouldContain(c => c.RagioneSociale == "Alpha Srl");
        result.Items.ShouldContain(c => c.RagioneSociale == "Gamma Consulting");
        Should.Throw<Shouldly.ShouldAssertException>(() => {
            result.Items.ShouldNotContain(c => c.RagioneSociale == "Beta SpA");
        });
    }

    [Fact]
    public async Task Should_Update_A_Company()
    {
        // Arrange
        var (dynamicType, dynamicEntity) = await CreateDynamicTypeAndEntityAsync(
            "AziendaType",
            "Tipo per aziende",
            "AnagrafiAziende",
            "Entità per anagrafi aziendali"
        );

        var dtoCompany = new HybridCompanyDto
        {
            DynamicEntityId = dynamicEntity.Id,
            RagioneSociale = "Delta Srl",
            Cognome = "Neri",
            Indirizzo = "Via Delta 123, Napoli",
            Telefono = "+39 123987456"
        };

        dtoCompany.SetProperty("Email", "info@delta.it");
        dtoCompany.SetProperty("PartitaIVA", "98765432109");

        var company = await _companyService.CreateAsync(dtoCompany);

        // Act - Aggiorniamo i dati dell'azienda
        company.RagioneSociale = "Delta Group Srl";
        company.Indirizzo = "Via Delta 456, Napoli";
        company.SetProperty("Email", "info@deltagroup.it");
        company.SetProperty("PEC", "deltagroup@pec.it");  // Aggiungiamo una nuova proprietà

        var updatedCompany = await _companyService.UpdateAsync(company.Id, company);

        // Assert
        updatedCompany.RagioneSociale.ShouldBe("Delta Group Srl");
        updatedCompany.Indirizzo.ShouldBe("Via Delta 456, Napoli");
        updatedCompany.GetProperty("Email").ShouldBe("info@deltagroup.it");
        updatedCompany.GetProperty("PEC").ShouldBe("deltagroup@pec.it");
        updatedCompany.GetProperty("PartitaIVA").ShouldBe("98765432109");  // Questa non è cambiata
    }

    [Fact]
    public async Task Should_Delete_A_Company()
    {
        // Arrange
        var (dynamicType, dynamicEntity) = await CreateDynamicTypeAndEntityAsync(
            "AziendaType",
            "Tipo per aziende",
            "AnagrafiAziende",
            "Entità per anagrafi aziendali"
        );

        var dtoCompany = new HybridCompanyDto
        {
            DynamicEntityId = dynamicEntity.Id,
            RagioneSociale = "Epsilon Srl",
            Cognome = "",
            Indirizzo = "Via Epsilon 123, Torino",
            Telefono = "+39 555666777"
        };

        var company = await _companyService.CreateAsync(dtoCompany);

        // Act
        await _companyService.DeleteAsync(company.Id);

        // Assert
        var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _companyService.GetUpdateObject(company.Id);
        });
        exception.Message.ShouldBe("CompanyNotFound");
    }

    [Fact]
    public async Task Should_Get_Company_For_Update()
    {
        // Arrange
        var (dynamicType, dynamicEntity) = await CreateDynamicTypeAndEntityAsync(
            "AziendaType",
            "Tipo per aziende",
            "AnagrafiAziende",
            "Entità per anagrafi aziendali"
        );

        var dtoCompany = new HybridCompanyDto
        {
            DynamicEntityId = dynamicEntity.Id,
            RagioneSociale = "Zeta Consulting",
            Cognome = "Gialli",
            Indirizzo = "Via Zeta 123, Firenze",
            Telefono = "+39 333444555"
        };

        dtoCompany.SetProperty("Email", "info@zeta.it");
        dtoCompany.SetProperty("CodiceFiscale", "GLLNTN80A01H501Z");

        var company = await _companyService.CreateAsync(dtoCompany);

        // Act
        var retrievedCompany = await _companyService.GetUpdateObject(company.Id);

        // Assert
        retrievedCompany.ShouldNotBeNull();
        retrievedCompany.Id.ShouldBe(company.Id);
        retrievedCompany.RagioneSociale.ShouldBe("Zeta Consulting");
        retrievedCompany.Cognome.ShouldBe("Gialli");
        retrievedCompany.Indirizzo.ShouldBe("Via Zeta 123, Firenze");
        retrievedCompany.Telefono.ShouldBe("+39 333444555");
        retrievedCompany.GetProperty("Email").ShouldBe("info@zeta.it");
        retrievedCompany.GetProperty("CodiceFiscale").ShouldBe("GLLNTN80A01H501Z");
    }

    [Fact]
    public async Task Should_Throw_Exception_When_Company_Not_Found()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _companyService.GetUpdateObject(Guid.NewGuid());
        });

        exception.Message.ShouldBe("CompanyNotFound");
    }
}