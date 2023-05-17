using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureFunctions.TestApplication.Application.SampleDomains;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.Interfaces
{
    public interface ISampleDomainsService : IDisposable
    {
        Task<Guid> CreateSampleDomain(SampleDomainCreateDto dto);
        Task<SampleDomainDto> FindSampleDomainById(Guid id);
        Task<List<SampleDomainDto>> FindSampleDomains();
        Task UpdateSampleDomain(Guid id, SampleDomainUpdateDto dto);
        Task DeleteSampleDomain(Guid id);
        Task<string> MappedAzureFunction(SampleMappedRequest request);
    }
}