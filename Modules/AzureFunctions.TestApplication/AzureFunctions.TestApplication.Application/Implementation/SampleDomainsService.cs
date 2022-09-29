using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AzureFunctions.TestApplication.Application.Interfaces;
using AzureFunctions.TestApplication.Application.SampleDomains;
using AzureFunctions.TestApplication.Domain;
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

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Guid> Create(SampleDomainCreateDTO dto)
        {
            var newSampleDomain = new SampleDomain
            {

            };

            _sampleDomainRepository.Add(newSampleDomain);
            await _sampleDomainRepository.UnitOfWork.SaveChangesAsync();
            return newSampleDomain.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<SampleDomainDTO> FindById(Guid id)
        {
            var element = await _sampleDomainRepository.FindByIdAsync(id);
            return element.MapToSampleDomainDTO(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<SampleDomainDTO>> FindAll()
        {
            var elements = await _sampleDomainRepository.FindAllAsync();
            return elements.MapToSampleDomainDTOList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Update(Guid id, SampleDomainUpdateDTO dto)
        {
            var existingSampleDomain = await _sampleDomainRepository.FindByIdAsync(id);

        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Delete(Guid id)
        {
            var existingSampleDomain = await _sampleDomainRepository.FindByIdAsync(id);
            _sampleDomainRepository.Remove(existingSampleDomain);
        }

        public void Dispose()
        {
        }
    }
}