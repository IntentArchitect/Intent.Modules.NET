using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.TestApplication.Application.Clients;
using Standard.AspNetCore.TestApplication.Application.Common.Pagination;
using Standard.AspNetCore.TestApplication.Application.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class ClientsService : IClientsService
    {
        [IntentManaged(Mode.Merge)]
        public ClientsService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Guid> CreateClient(ClientCreate dto, CancellationToken cancellationToken = default)
        {
            // TODO: Implement CreateClient (ClientsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Client> FindClientById(Guid id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindClientById (ClientsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<Client>> FindClients(CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindClients (ClientsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<PagedResult<Client>> FindClientsPaged(
            int pageNo,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindClientsPaged (ClientsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task UpdateClient(Guid id, ClientUpdate dto, CancellationToken cancellationToken = default)
        {
            // TODO: Implement UpdateClient (ClientsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task DeleteClient(Guid id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement DeleteClient (ClientsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}