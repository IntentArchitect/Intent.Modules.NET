using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Companies;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Interfaces;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Entities;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class CompaniesService : ICompaniesService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public CompaniesService(ICompanyRepository companyRepository, IMapper mapper)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreateCompany(CompanyCreateDto dto, CancellationToken cancellationToken = default)
        {
            var company = new Company(
                name: dto.Name,
                contactDetailsVOS: dto.ContactDetailsVOS
                    .Select(dvos => new ContactDetailsVO(
                        cell: dvos.Cell,
                        email: dvos.Email))
                    .ToList());

            _companyRepository.Add(company);
            await _companyRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return company.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CompanyDto> FindCompanyById(Guid id, CancellationToken cancellationToken = default)
        {
            var company = await _companyRepository.FindByIdAsync(id, cancellationToken);
            if (company is null)
            {
                throw new NotFoundException($"Could not find Company '{id}'");
            }
            return company.MapToCompanyDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<CompanyDto>> FindCompanies(CancellationToken cancellationToken = default)
        {
            var companies = await _companyRepository.FindAllAsync(cancellationToken);
            return companies.MapToCompanyDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteCompany(Guid id, CancellationToken cancellationToken = default)
        {
            var company = await _companyRepository.FindByIdAsync(id, cancellationToken);
            if (company is null)
            {
                throw new NotFoundException($"Could not find Company '{id}'");
            }

            _companyRepository.Remove(company);
        }

        public void Dispose()
        {
        }
    }
}