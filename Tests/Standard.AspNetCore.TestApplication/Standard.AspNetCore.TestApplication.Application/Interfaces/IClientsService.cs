using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.TestApplication.Application.Clients;
using Standard.AspNetCore.TestApplication.Application.Common.Pagination;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.Interfaces
{
    public interface IClientsService : IDisposable
    {
        Task<Guid> CreateClient(ClientCreate dto, CancellationToken cancellationToken = default);
        Task<Client> FindClientById(Guid id, CancellationToken cancellationToken = default);
        Task<List<Client>> FindClients(CancellationToken cancellationToken = default);
        Task<PagedResult<Client>> FindClientsPaged(int pageNo, int pageSize, CancellationToken cancellationToken = default);
        Task UpdateClient(Guid id, ClientUpdate dto, CancellationToken cancellationToken = default);
        Task DeleteClient(Guid id, CancellationToken cancellationToken = default);
    }
}