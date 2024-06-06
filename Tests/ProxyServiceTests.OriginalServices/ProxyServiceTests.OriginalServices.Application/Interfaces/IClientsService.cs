using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using ProxyServiceTests.OriginalServices.Application.Clients;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace ProxyServiceTests.OriginalServices.Application.Interfaces
{
    public interface IClientsService : IDisposable
    {
        Task<Guid> CreateClient(ClientCreateDto dto, CancellationToken cancellationToken = default);
        Task<ClientDto> FindClientById(Guid id, CancellationToken cancellationToken = default);
        Task<List<ClientDto>> FindClients(CancellationToken cancellationToken = default);
        Task UpdateClient(Guid id, ClientUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteClient(Guid id, CancellationToken cancellationToken = default);
    }
}