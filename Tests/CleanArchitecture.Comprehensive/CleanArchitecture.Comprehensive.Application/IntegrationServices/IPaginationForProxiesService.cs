using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.IntegrationServices.Contracts;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.IntegrationServices
{
    public interface IPaginationForProxiesService : IDisposable
    {
        Task<PagedResult<string>> PaginatedResultAsync(int pageNo, int pageSize, CancellationToken cancellationToken = default);
    }
}