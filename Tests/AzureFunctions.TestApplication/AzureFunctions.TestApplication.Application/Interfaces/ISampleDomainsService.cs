using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.TestApplication.Application.Common.Pagination;
using AzureFunctions.TestApplication.Application.SampleDomains;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.Interfaces
{
    public interface ISampleDomainsService : IDisposable
    {
        Task<Guid> CreateSampleDomain(SampleDomainCreateDto dto, CancellationToken cancellationToken = default);
        Task<SampleDomainDto> FindSampleDomainById(Guid id, CancellationToken cancellationToken = default);
        Task<List<SampleDomainDto>> FindSampleDomains(CancellationToken cancellationToken = default);
        Task UpdateSampleDomain(Guid id, SampleDomainUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteSampleDomain(Guid id, CancellationToken cancellationToken = default);
        Task<string> MappedAzureFunction(SampleMappedRequest request, CancellationToken cancellationToken = default);
        Task<PagedResult<SampleDomainDto>> FindSampleDomainsPaged(int pageNo, int pageSize, CancellationToken cancellationToken = default);
    }
}