using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.TestApplication.Application.IntegrationServices.Contracts;
using AzureFunctions.TestApplication.Application.IntegrationServices.Contracts.Services.SampleDomains;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "2.0")]

namespace AzureFunctions.TestApplication.Application.IntegrationServices
{
    public interface ISampleDomainsService : IDisposable
    {
        Task<Guid> CreateSampleDomainAsync(SampleDomainCreateDto dto, CancellationToken cancellationToken = default);
        Task<SampleDomainDto> FindSampleDomainByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<SampleDomainDto>> FindSampleDomainsAsync(CancellationToken cancellationToken = default);
        Task UpdateSampleDomainAsync(Guid id, SampleDomainUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteSampleDomainAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PagedResult<SampleDomainDto>> FindSampleDomainsPagedAsync(int pageNo, int pageSize, CancellationToken cancellationToken = default);
    }
}