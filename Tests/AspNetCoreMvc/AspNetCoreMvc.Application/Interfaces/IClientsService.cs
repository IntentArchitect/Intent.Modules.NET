using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreMvc.Application.ClientsService;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AspNetCoreMvc.Application.Interfaces
{
    public interface IClientsService
    {
        Task<Guid> CreateClient(string groupId, ClientCreateDto dto, CancellationToken cancellationToken = default);
        Task<ClientDto> FindClientById(string groupId, Guid id, CancellationToken cancellationToken = default);
        Task<List<ClientDto>> FindClients(string groupId, CancellationToken cancellationToken = default);
        Task UpdateClient(string groupId, Guid id, ClientUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteClient(string groupId, Guid id, CancellationToken cancellationToken = default);
        Task<List<ClientDto>> FindClientsWithoutView(string groupId, CancellationToken cancellationToken = default);
    }
}