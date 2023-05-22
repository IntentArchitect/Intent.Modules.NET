using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AzureFunctions.TestApplication.Application.Interfaces;
using AzureFunctions.TestApplication.Application.SampleDomains;
using AzureFunctions.TestApplication.Domain.Entities;
using AzureFunctions.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class SampleDomainsService : ISampleDomainsService
    {
        private readonly ISampleDomainRepository _sampleDomainRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public SampleDomainsService(ISampleDomainRepository sampleDomainRepository, IMapper mapper)
        {
            _sampleDomainRepository = sampleDomainRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreateSampleDomain(
            SampleDomainCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            var newSampleDomain = new SampleDomain
            {
                Attribute = dto.Attribute,
            };
            _sampleDomainRepository.Add(newSampleDomain);
            await _sampleDomainRepository.UnitOfWork.SaveChangesAsync();
            return newSampleDomain.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<SampleDomainDto> FindSampleDomainById(Guid id, CancellationToken cancellationToken = default)
        {
            var element = await _sampleDomainRepository.FindByIdAsync(id);
            return element.MapToSampleDomainDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<SampleDomainDto>> FindSampleDomains(CancellationToken cancellationToken = default)
        {
            var elements = await _sampleDomainRepository.FindAllAsync();
            return elements.MapToSampleDomainDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateSampleDomain(
            Guid id,
            SampleDomainUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            var existingSampleDomain = await _sampleDomainRepository.FindByIdAsync(id);
            existingSampleDomain.Attribute = dto.Attribute;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteSampleDomain(Guid id, CancellationToken cancellationToken = default)
        {
            var existingSampleDomain = await _sampleDomainRepository.FindByIdAsync(id);
            _sampleDomainRepository.Remove(existingSampleDomain);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<string> MappedAzureFunction(
            SampleMappedRequest request,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        public void Dispose()
        {
        }
    }
}