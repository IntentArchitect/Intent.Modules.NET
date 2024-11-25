using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreMvc.Application.Clients;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AspNetCoreMvc.Application.Interfaces
{
    public interface IClientsService
    {
        Task<Guid> CreateClient(ClientCreateDto dto, CancellationToken cancellationToken = default);
        Task<ClientDto> FindClientById(Guid id, CancellationToken cancellationToken = default);
        Task<List<ClientDto>> FindClients(CancellationToken cancellationToken = default);
        Task UpdateClient(Guid id, ClientUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteClient(Guid id, CancellationToken cancellationToken = default);
        Task<List<ClientDto>> FindClientsWithoutView(CancellationToken cancellationToken = default);
    }
}