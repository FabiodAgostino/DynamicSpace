using Infocad.DynamicSpace.DynamicAttirbutes;
using Infocad.DynamicSpace.DynamicAttributes;
using Infocad.DynamicSpace.DynamicEntities;
using Infocad.DynamicSpace.DynamicFormats;
using Infocad.DynamicSpace.DynamicRules;
using Infocad.DynamicSpace.DynamicTypes;
using Infocad.DynamicSpace.HybridBuildings;
using Infocad.DynamicSpace.HybridCompanies;
using Infocad.DynamicSpace.Totems;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infocad.DynamicSpace.DynamicHierarchies;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;

namespace Infocad.DynamicSpace
{
    public class DynamicSpaceTestDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly ICurrentTenant _currentTenant;
        private readonly IRepository<DynamicType, Guid> _repositoryDynamicType;
        private readonly IRepository<DynamicAttribute, Guid> _repositoryDynamicAttribute;
        private readonly IRepository<DynamicEntity, Guid> _repositoryDynamicEntity;
        private readonly IRepository<DynamicFormat, Guid> _repositoryDynamicFormat;
        private readonly IRepository<DynamicRule, Guid> _repositoryDynamicRule;
        private readonly IRepository<HybridCompany, Guid> _repositoryCompany;
        private readonly IRepository<HybridBuilding, Guid> _repositoryBuilding;
        private readonly IRepository<Totem, Guid> _repositoryTotem;
        private readonly IRepository<DynamicHierarchy,Guid> _repositoryDynamicHierarchy;

        public DynamicSpaceTestDataSeedContributor(
            IRepository<DynamicType, Guid> repositoryDynamicType,
            IRepository<DynamicAttribute, Guid> repositoryDynamicAttribute,
            IRepository<DynamicEntity, Guid> repositoryDynamicEntity,
            IRepository<DynamicFormat, Guid> repositoryDynamicFormat,
            IRepository<DynamicRule, Guid> repositoryDynamicRule,
            IRepository<HybridCompany, Guid> repositoryCompany,
            IRepository<HybridBuilding, Guid> repositoryHybridBuilding,
            IRepository<Totem, Guid> repositoryTotem,
            IRepository<DynamicHierarchy,Guid> repositoryDynamicHierarchy,

            ICurrentTenant currentTenant)
        {
            _currentTenant = currentTenant;
            _repositoryDynamicType = repositoryDynamicType;
            _repositoryDynamicAttribute = repositoryDynamicAttribute;
            _repositoryDynamicEntity = repositoryDynamicEntity;
            _repositoryDynamicFormat = repositoryDynamicFormat;
            _repositoryDynamicRule = repositoryDynamicRule;
            _repositoryDynamicHierarchy = repositoryDynamicHierarchy;
            _repositoryCompany = repositoryCompany;
            _repositoryBuilding = repositoryHybridBuilding;
            _repositoryTotem = repositoryTotem;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            DynamicType Type = new();
            // Seed per DynamicType
            if (await _repositoryDynamicType.GetCountAsync() <= 0)
            {
                Type = new DynamicType
                {
                    Name = "Entity 1",
                    Description = "Entity 1 Description",
                };
                await _repositoryDynamicType.InsertAsync(Type);

                var Type2 = new DynamicType()
                {
                    Name = "Entity 2",
                    Description = "Entity 2 Description"
                };
                await _repositoryDynamicType.InsertAsync(Type2);
            }

            // Seed per DynamicAttribute
            Guid IdAttribute = Guid.Empty;
            if (await _repositoryDynamicAttribute.GetCountAsync() <= 0)
            {
                var attribute = await _repositoryDynamicAttribute.InsertAsync(new DynamicAttribute
                {
                    Name = "Attribute 1",
                    Description = "Attribute 1 Description",
                    Type = DynamicAttributeType.Text,
                });
                IdAttribute = attribute.Id;
                await _repositoryDynamicAttribute.InsertAsync(new DynamicAttribute
                {
                    Name = "Attribute 2",
                    Description = "Attribute 2 Description",
                    Type = DynamicAttributeType.Number,
                });
            }

            Guid IdFormat = Guid.Empty;
            //seed per DynamicFormat
            if (await _repositoryDynamicFormat.GetCountAsync() <= 0)
            {
                // Creazione del formato per le lettere maiuscole
                var upperCaseFormat = new DynamicFormat(
                    Guid.NewGuid(),
                    "Lettere Maiuscole",
                    DynamicAttributeType.Text,
                    "Formato che visualizza il testo tutto in maiuscolo",
                    "^[A-Z]*$"  // Regex per lettere maiuscole (formato di visualizzazione)
                );

                var format = await _repositoryDynamicFormat.InsertAsync(upperCaseFormat);
                IdFormat = format.Id;
            }

            Guid IdRule = Guid.Empty;
            // Seed per DynamicRule
            if (await _repositoryDynamicRule.GetCountAsync() <= 0)
            {
                // Email validation
                var emailRule = new DynamicRule
                {
                    Name = "Email Validation",
                    Description = "Validazione formato email",
                    Rule = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
                    AttributeType = DynamicAttributeType.Text
                };
                var rule1 = await _repositoryDynamicRule.InsertAsync(emailRule);
                IdRule = rule1.Id;

                // Phone number validation
                var phoneRule = new DynamicRule
                {
                    Name = "Phone Validation",
                    Description = "Validazione numero di telefono",
                    Rule = @"^(\+\d{1,3}[- ]?)?\d{10}$",
                    AttributeType = DynamicAttributeType.Text
                };
                await _repositoryDynamicRule.InsertAsync(phoneRule);

                // Italian tax code validation
                var taxCodeRule = new DynamicRule
                {
                    Name = "Codice Fiscale",
                    Description = "Validazione Codice Fiscale italiano",
                    Rule = @"^[A-Z]{6}[0-9]{2}[A-Z][0-9]{2}[A-Z][0-9]{3}[A-Z]$",
                    AttributeType = DynamicAttributeType.Text
                };
                await _repositoryDynamicRule.InsertAsync(taxCodeRule);

                // VAT number validation
                var vatRule = new DynamicRule
                {
                    Name = "Partita IVA",
                    Description = "Validazione Partita IVA italiana",
                    Rule = @"^[0-9]{11}$",
                    AttributeType = DynamicAttributeType.Number
                };
                await _repositoryDynamicRule.InsertAsync(vatRule);

                // Alphanumeric only
                var alphanumericRule = new DynamicRule
                {
                    Name = "Alphanumeric Only",
                    Description = "Solo caratteri alfanumerici",
                    Rule = @"^[A-Za-z0-9]+$",
                    AttributeType = DynamicAttributeType.Text
                };
                await _repositoryDynamicRule.InsertAsync(alphanumericRule);

                // Italian license plate
                var licensePlateRule = new DynamicRule
                {
                    Name = "Targa IT",
                    Description = "Validazione targa italiana",
                    Rule = @"^[A-Z]{2}[0-9]{3}[A-Z]{2}$",
                    AttributeType = DynamicAttributeType.Text
                };
                await _repositoryDynamicRule.InsertAsync(licensePlateRule);

                // Date format validation
                var dateRule = new DynamicRule
                {
                    Name = "Date DD/MM/YYYY",
                    Description = "Validazione formato data DD/MM/YYYY",
                    Rule = @"^(0[1-9]|[12][0-9]|3[01])/(0[1-9]|1[0-2])/\d{4}$",
                    AttributeType = DynamicAttributeType.DateTime
                };
                await _repositoryDynamicRule.InsertAsync(dateRule);

                // Password strength validation
                var passwordRule = new DynamicRule
                {
                    Name = "Strong Password",
                    Description = "Password forte: minimo 8 caratteri, una maiuscola, una minuscola, un numero e un carattere speciale",
                    Rule = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
                    AttributeType = DynamicAttributeType.Text
                };
                await _repositoryDynamicRule.InsertAsync(passwordRule);
            }

            // Seed per DynamicEntity
            Guid entityId = Guid.Empty;
            Guid entityIdBuilding = Guid.Empty;

            if (await _repositoryDynamicEntity.GetCountAsync() <= 0)
            {
                if (Type != null)
                {
                    var entity = new DynamicEntity
                    {
                        Name = "Entity 1",
                        Description = "Dynamic Entity 1 seeded",
                        DynamicTypeId = Type.Id,
                    };
                    entity.Attributes = new List<DynamicEntityAttribute>() {
                        new DynamicEntityAttribute(IdAttribute, 0, "Label", string.Empty, false, IdFormat, new Guid()), 
                    };
                    await _repositoryDynamicEntity.InsertAsync(entity);

                    // Creiamo un'entità che utilizza una regola
                    var entity2 = new DynamicEntity
                    {
                        Name = "Entity 2",
                        Description = "Dynamic Entity 2 with rule",
                        DynamicTypeId = Type.Id,
                    };

                    // Aggiungiamo un attributo con una regola
                    var attributeWithRule = new DynamicEntityAttribute(IdAttribute, 0, "Email Field", string.Empty, false, IdFormat, new Guid());
                    attributeWithRule.DynamicRuleId = IdRule; // Assegniamo la regola di email validation

                    entity2.Attributes = new List<DynamicEntityAttribute>() { attributeWithRule };
                    await _repositoryDynamicEntity.InsertAsync(entity2);

                    var companyEntity = new DynamicEntity
                    {
                        Name = "Anagrafica Aziende",
                        Description = "Entità per la gestione delle anagrafiche aziendali",
                        DynamicTypeId = Type.Id, // Utilizziamo lo stesso tipo per semplicità
                    };

                    companyEntity.Attributes = new List<DynamicEntityAttribute>() {
                        new DynamicEntityAttribute(IdAttribute, 0, "Ragione Sociale", string.Empty, false, IdFormat, new Guid()),
                        new DynamicEntityAttribute(IdAttribute, 1, "Telefono", string.Empty, false, IdFormat, new Guid()),
                        new DynamicEntityAttribute(IdAttribute, 2, "Indirizzo", string.Empty, false, IdFormat, new Guid()),
                    };

                    var companyEntityCreated = await _repositoryDynamicEntity.InsertAsync(companyEntity);
                    entityId = companyEntity.Id;


                    var buildingEntity = new DynamicEntity
                    {
                        Name = "Edifici",
                        Description = "Entità per la gestione degli edifici",
                        DynamicTypeId = Type.Id, // Utilizziamo lo stesso tipo per semplicità
                    };

                    buildingEntity.Attributes = new List<DynamicEntityAttribute>() {
                        new DynamicEntityAttribute(IdAttribute, 0, "X", string.Empty, false, IdFormat, new Guid()),
                        new DynamicEntityAttribute(IdAttribute, 1, "Y", string.Empty, false, IdFormat, new Guid()),
                        new DynamicEntityAttribute(IdAttribute, 2, "Name", string.Empty, false, IdFormat, new Guid()),
                    };

                    var buildingEntityCreated = await _repositoryDynamicEntity.InsertAsync(companyEntity);
                    entityIdBuilding = buildingEntityCreated.Id;
                }
            }
            //seed HybridCompany
            if (await _repositoryCompany.GetCountAsync() <= 0)
            {
                // Creiamo alcune aziende di esempio
                var company1 = new HybridCompany
                {
                    DynamicEntityId = entityId, // Associamo l'entità creata in precedenza
                    RagioneSociale = "Infocad Srl",
                    Cognome = "", // Non applicabile per questa azienda
                    Indirizzo = "Via Roma 123, Foligno",
                    Telefono = "+39 0742 123456"
                };

                // Aggiungiamo proprietà extra
                company1.ExtraProperties.Add("Email", "info@infocad.it");
                company1.ExtraProperties.Add("PEC", "infocad@pec.it");
                company1.ExtraProperties.Add("PartitaIVA", "01234567890");
                company1.ExtraProperties.Add("Sito", "www.infocad.it");

                await _repositoryCompany.InsertAsync(company1);

                var company2 = new HybridCompany
                {
                    DynamicEntityId = entityId, // Associamo l'entità creata in precedenza
                    RagioneSociale = "Rossi Engineering",
                    Cognome = "Rossi", // In questo caso abbiamo anche il cognome
                    Indirizzo = "Via Verdi 456, Perugia",
                    Telefono = "+39 075 789012"
                };

                // Aggiungiamo proprietà extra
                company2.ExtraProperties.Add("Email", "info@rossiengineering.it");
                company2.ExtraProperties.Add("PEC", "rossiengineering@pec.it");
                company2.ExtraProperties.Add("PartitaIVA", "12345678901");
                company2.ExtraProperties.Add("CodiceFiscale", "RSSGVN80A01G478Z");
                company2.ExtraProperties.Add("Sito", "www.rossiengineering.it");

                await _repositoryCompany.InsertAsync(company2);

                var company3 = new HybridCompany
                {
                    DynamicEntityId = entityId,
                    RagioneSociale = "Bianchi Software",
                    Cognome = "Bianchi",
                    Indirizzo = "Corso Mazzini 789, Assisi",
                    Telefono = "+39 075 345678"
                };

                company3.ExtraProperties.Add("Email", "info@bianchisoftware.it");
                company3.ExtraProperties.Add("PartitaIVA", "23456789012");

                await _repositoryCompany.InsertAsync(company3);
            }

            //seed HybridBuilding
            if (await _repositoryBuilding.GetCountAsync() <= 0)
            {
                var building1 = new HybridBuilding
                {
                    DynamicEntityId = entityIdBuilding, 
                    X = 100f,
                    Y = 200f, 
                    Name = "Edificio A",
                };

                // Aggiungiamo proprietà extra
                building1.ExtraProperties.Add("GrandezzaMq", 350);
                building1.ExtraProperties.Add("Altezza", 20);
                building1.ExtraProperties.Add("LavoriInCorso", true);

                await _repositoryBuilding.InsertAsync(building1);

                var building2 = new HybridBuilding
                {
                    DynamicEntityId = entityIdBuilding,
                    X = 200f,
                    Y = 300f,
                    Name = "Edificio B",
                };

                // Aggiungiamo proprietà extra
                building2.ExtraProperties.Add("GrandezzaMq", 300);
                building2.ExtraProperties.Add("Altezza", 30);
                building2.ExtraProperties.Add("LavoriInCorso", false);

                await _repositoryBuilding.InsertAsync(building2);

                var building3 = new HybridBuilding
                {
                    DynamicEntityId = entityIdBuilding,
                    X = 211f,
                    Y = 302f,
                    Name = "Edificio C",
                };

                // Aggiungiamo proprietà extra
                building3.ExtraProperties.Add("GrandezzaMq", 301);
                building3.ExtraProperties.Add("Altezza", 40);
                building3.ExtraProperties.Add("LavoriInCorso", false);

                await _repositoryBuilding.InsertAsync(building3);
            }

            if (await _repositoryTotem.GetCountAsync() <= 0)
            {
                var totem1 = new Totem
                {
                    Name = "Totem Ingresso",
                    Description = "Totem informativo posizionato all'ingresso principale",
                    X = 50,
                    Y = 100
                };
                await _repositoryTotem.InsertAsync(totem1);

                var totem2 = new Totem
                {
                    Name = "Totem Reception",
                    Description = "Totem per informazioni e registrazione ospiti",
                    X = 150,
                    Y = 200
                };
                await _repositoryTotem.InsertAsync(totem2);

                var totem3 = new Totem
                {
                    Name = "Totem Mensa",
                    Description = "Totem con menu e orari della mensa aziendale",
                    X = 300,
                    Y = 150
                };
                await _repositoryTotem.InsertAsync(totem3);

                var totem4 = new Totem
                {
                    Name = "Totem Emergenza",
                    Description = "Totem con procedure di emergenza e vie di fuga",
                    X = 0,
                    Y = 0
                };
                await _repositoryTotem.InsertAsync(totem4);

                var totem5 = new Totem
                {
                    Name = "Totem Parcheggio",
                    Description = "Totem per informazioni parcheggio e viabilità",
                    X = -25,
                    Y = -50
                };
                await _repositoryTotem.InsertAsync(totem5);
            }
        }
    }
}