using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using ProxyServiceTests.Proxy.PTH.Application.IntegrationServices.Contracts.OriginalServices.Services.Clients;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "2.0")]

namespace ProxyServiceTests.Proxy.PTH.Application.IntegrationServices
{
    public interface IClientsService : IDisposable
    {
        Task<Guid> CreateClientAsync(ClientCreateDto dto, CancellationToken cancellationToken = default);
        Task<ClientDto> FindClientByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<ClientDto>> FindClientsAsync(CancellationToken cancellationToken = default);
        Task UpdateClientAsync(Guid id, ClientUpdateDto dto, CancellationToken cancellationToken = default);
    }
}