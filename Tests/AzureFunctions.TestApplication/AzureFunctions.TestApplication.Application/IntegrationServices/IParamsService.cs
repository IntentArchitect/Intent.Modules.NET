using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "2.0")]

namespace AzureFunctions.TestApplication.Application.IntegrationServices
{
    public interface IParamsService : IDisposable
    {
        Task FromBodyTestAsync(List<int> ids, CancellationToken cancellationToken = default);
        Task<int> GetByIdsHeadersTestAsync(List<int> ids, CancellationToken cancellationToken = default);
        Task<int> GetByIdsQueryTestAsync(List<int> ids, CancellationToken cancellationToken = default);
    }
}