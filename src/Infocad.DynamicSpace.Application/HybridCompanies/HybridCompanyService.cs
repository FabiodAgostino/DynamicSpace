using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Infocad.DynamicSpace.HybridCompanies
{
    public class HybridCompanyService : DynamicSpaceAppService, IHybridCompanyService
    {
        private readonly IHybridCompanyRepository _companyRepository;

        public HybridCompanyService(IHybridCompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public async Task<PagedResultDto<HybridCompanyDto>> GetListObjects(Guid? idEntity,
            GetHybridCompanyListDto input)
        {
            var companies = await _companyRepository.GetListByEntityAsync(idEntity,
                input.SkipCount,
                input.MaxResultCount,
                input.Sorting,
                input.Filter
            );

            var totalCount = input.Filter == null
                ? await _companyRepository.CountAsync()
                : await _companyRepository.CountAsync(
                    company => company.ExtraProperties.ContainsValue(input.Filter) ||
                               company.RagioneSociale.Contains(input.Filter) ||
                               company.Cognome.Contains(input.Filter) ||
                               company.Indirizzo.Contains(input.Filter) ||
                               company.Telefono.Contains(input.Filter));

            return new PagedResultDto<HybridCompanyDto>(
                totalCount,
                ObjectMapper.Map<List<HybridCompany>, List<HybridCompanyDto>>(companies));
        }

        public async Task<HybridCompanyDto> CreateAsync(HybridCompanyDto input)
        {
            var company = ObjectMapper.Map<HybridCompanyDto, HybridCompany>(input);
            var retval = await _companyRepository.InsertAsync(company);
            return ObjectMapper.Map<HybridCompany, HybridCompanyDto>(retval);
        }

        public async Task<HybridCompanyDto> UpdateAsync(Guid id, HybridCompanyDto input)
        {
            try
            {
                var company = await _companyRepository.GetAsync(id);

                // Aggiornamento proprietà specifiche
                company.RagioneSociale = input.RagioneSociale;
                company.Cognome = input.Cognome;
                company.Indirizzo = input.Indirizzo;
                company.Telefono = input.Telefono;
                company.DynamicEntityId = input.DynamicEntityId;

                // Aggiornamento ExtraProperties
                foreach (var value in input.ExtraProperties)
                {
                    if (company.ExtraProperties.ContainsKey(value.Key))
                    {
                        company.ExtraProperties[value.Key] = value.Value;
                    }
                    else
                    {
                        company.ExtraProperties.Add(value.Key, value.Value);
                    }
                }

                await _companyRepository.UpdateAsync(company);
                return ObjectMapper.Map<HybridCompany, HybridCompanyDto>(company);
            }
            catch (EntityNotFoundException ex)
            {
                throw new UserFriendlyException(L["CompanyNotFound", id]);
            }
        }

        public async Task<List<HybridCompanyDto>> GetEntities()
        {
            var companies = await _companyRepository.GetListAsync();
            return ObjectMapper.Map<List<HybridCompany>, List<HybridCompanyDto>>(companies);
        }

        public async Task<HybridCompanyDto> DeleteAsync(Guid id)
        {
            try
            {
                var company = await _companyRepository.GetAsync(id);
                await _companyRepository.DeleteAsync(company);
                return ObjectMapper.Map<HybridCompany, HybridCompanyDto>(company);
            }
            catch (EntityNotFoundException ex)
            {
                throw new UserFriendlyException(L["CompanyNotFound", id]);
            }
        }

        public async Task<HybridCompanyDto> GetUpdateObject(Guid id)
        {
            try
            {
                var company = await _companyRepository.GetAsync(id);
                return ObjectMapper.Map<HybridCompany, HybridCompanyDto>(company);
            }
            catch (EntityNotFoundException ex)
            {
                throw new UserFriendlyException(L["CompanyNotFound", id]);
            }
        }

        public Task<PagedResultDto<HybridCompanyDto>> GetListObjectsByEntityAsync<Y>(Guid idEntity, Y input)
        {
            throw new NotImplementedException();
        }

        public async Task<HybridCompanyDto> GetById(Guid id)
        {
            try
            {
                var company = await _companyRepository.GetAsync(id);
                return ObjectMapper.Map<HybridCompany, HybridCompanyDto>(company);
            }
            catch (EntityNotFoundException ex)
            {
                throw new UserFriendlyException(L["CompanyNotFound", id]);
            }
        }

    }
}
