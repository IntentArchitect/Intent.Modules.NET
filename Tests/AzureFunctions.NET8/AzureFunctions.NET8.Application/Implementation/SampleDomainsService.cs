using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AzureFunctions.NET8.Application.Common.Pagination;
using AzureFunctions.NET8.Application.Interfaces;
using AzureFunctions.NET8.Application.SampleDomains;
using AzureFunctions.NET8.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AzureFunctions.NET8.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class SampleDomainsService : ISampleDomainsService
    {
        private readonly ISampleDomainRepository _sampleDomainRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public SampleDomainsService(ISampleDomainRepository sampleDomainRepository, IMapper mapper)
        {
            _sampleDomainRepository = sampleDomainRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<Guid> CreateSampleDomain(
            SampleDomainCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement CreateSampleDomain (SampleDomainsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<SampleDomainDto> FindSampleDomainById(Guid id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindSampleDomainById (SampleDomainsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<List<SampleDomainDto>> FindSampleDomains(CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindSampleDomains (SampleDomainsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task UpdateSampleDomain(
            Guid id,
            SampleDomainUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement UpdateSampleDomain (SampleDomainsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task DeleteSampleDomain(Guid id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement DeleteSampleDomain (SampleDomainsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<string> MappedAzureFunction(
            SampleMappedRequest request,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement MappedAzureFunction (SampleDomainsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<PagedResult<SampleDomainDto>> FindSampleDomainsPaged(
            int pageNo,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindSampleDomainsPaged (SampleDomainsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        /// <summary>
        /// This should print out a warning to say that the query should also be set to a collection since the service operation expects to return a collection
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<SampleDomainDto>> FindByNameForSingleSampleDomainMapToCollection(
            string name,
            CancellationToken cancellationToken = default)
        {
            var entity = await _sampleDomainRepository.FindAllAsync(x => x.Name == name, cancellationToken);
            return entity.MapToSampleDomainDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<SampleDomainDto>> FindSampleDomainsByName(
            string name,
            CancellationToken cancellationToken = default)
        {
            var entity = await _sampleDomainRepository.FindAllAsync(x => x.Name == name, cancellationToken);
            return entity.MapToSampleDomainDtoList(_mapper);
        }

        /// <summary>
        /// This is not meant to be implemented automatically
        /// </summary>
        /// <param name="attribute">Comment for this parameter</param>
        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<SampleDomainDto> FindSampleDomainByAttribute(
            string attribute,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindSampleDomainByAttribute (SampleDomainsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}