using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Dapr.Application.Clients;
using CleanArchitecture.Dapr.Application.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class ClientsService : IClientsService
    {
        [IntentManaged(Mode.Merge)]
        public ClientsService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<string> CreateClient(ClientCreateDto dto, CancellationToken cancellationToken = default)
        {
            // TODO: Implement CreateClient (ClientsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<ClientDto> FindClientById(string id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindClientById (ClientsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<ClientDto>> FindClients(CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindClients (ClientsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task UpdateClient(string id, ClientUpdateDto dto, CancellationToken cancellationToken = default)
        {
            // TODO: Implement UpdateClient (ClientsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task DeleteClient(string id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement DeleteClient (ClientsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        public void Dispose()
        {
        }
    }
}