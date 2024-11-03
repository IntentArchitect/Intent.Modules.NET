using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AzureFunctions.NET8.Application.Common.Pagination;
using AzureFunctions.NET8.Application.Interfaces;
using AzureFunctions.NET8.Application.SampleDomains;
using AzureFunctions.NET8.Domain.Common.Exceptions;
using AzureFunctions.NET8.Domain.Entities;
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

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreateSampleDomain(
            SampleDomainCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            var newSampleDomain = new SampleDomain
            {
                Attribute = dto.Attribute,
                Name = dto.Name,
            };
            _sampleDomainRepository.Add(newSampleDomain);
            await _sampleDomainRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newSampleDomain.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<SampleDomainDto> FindSampleDomainById(Guid id, CancellationToken cancellationToken = default)
        {
            var element = await _sampleDomainRepository.FindByIdAsync(id, cancellationToken);

            if (element is null)
            {
                throw new NotFoundException($"Could not find SampleDomain {id}");
            }
            return element.MapToSampleDomainDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<SampleDomainDto>> FindSampleDomains(CancellationToken cancellationToken = default)
        {
            var elements = await _sampleDomainRepository.FindAllAsync(cancellationToken);
            return elements.MapToSampleDomainDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateSampleDomain(
            Guid id,
            SampleDomainUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            var existingSampleDomain = await _sampleDomainRepository.FindByIdAsync(id, cancellationToken);

            if (existingSampleDomain is null)
            {
                throw new NotFoundException($"Could not find SampleDomain {id}");
            }
            existingSampleDomain.Attribute = dto.Attribute;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteSampleDomain(Guid id, CancellationToken cancellationToken = default)
        {
            var existingSampleDomain = await _sampleDomainRepository.FindByIdAsync(id, cancellationToken);

            if (existingSampleDomain is null)
            {
                throw new NotFoundException($"Could not find SampleDomain {id}");
            }
            _sampleDomainRepository.Remove(existingSampleDomain);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<string> MappedAzureFunction(
            SampleMappedRequest request,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement MappedAzureFunction (SampleDomainsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<PagedResult<SampleDomainDto>> FindSampleDomainsPaged(
            int pageNo,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var results = await _sampleDomainRepository.FindAllAsync(
                pageNo: pageNo,
                pageSize: pageSize,
                cancellationToken: cancellationToken);
            return results.MapToPagedResult(x => x.MapToSampleDomainDto(_mapper));
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
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<SampleDomainDto> FindSampleDomainByAttribute(
            string attribute,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindSampleDomainByAttribute (SampleDomainsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}