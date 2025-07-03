using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Dapr.Application.Clients;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.Interfaces
{
    public interface IClientsService
    {
        Task<string> CreateClient(ClientCreateDto dto, CancellationToken cancellationToken = default);
        Task<ClientDto> FindClientById(string id, CancellationToken cancellationToken = default);
        Task<List<ClientDto>> FindClients(CancellationToken cancellationToken = default);
        Task UpdateClient(string id, ClientUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteClient(string id, CancellationToken cancellationToken = default);
    }
}