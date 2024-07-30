using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using ProxyServiceTests.Proxy.AuthHeaderProvider.Application.ClientsServices;
using ProxyServiceTests.Proxy.AuthHeaderProvider.Application.IntegrationServices.Contracts.OriginalServices.Services.Clients;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace ProxyServiceTests.Proxy.AuthHeaderProvider.Application.Interfaces
{
    public interface IClientsService : IDisposable
    {
        Task<Guid> CreateClient(ClientsServices.ClientCreateDto dto, CancellationToken cancellationToken = default);
        Task<ClientDto> FindClientById(Guid id, CancellationToken cancellationToken = default);
        Task<List<ClientDto>> FindClients(CancellationToken cancellationToken = default);
        Task UpdateClient(Guid id, ClientsServices.ClientUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteClient(Guid id, CancellationToken cancellationToken = default);
    }
}