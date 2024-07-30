using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using ProxyServiceTests.Proxy.AuthHeaderProvider.Application.ClientsServices;
using ProxyServiceTests.Proxy.AuthHeaderProvider.Application.IntegrationServices;
using ProxyServiceTests.Proxy.AuthHeaderProvider.Application.IntegrationServices.Contracts.OriginalServices.Services.Clients;
using ProxyServiceTests.Proxy.AuthHeaderProvider.Application.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace ProxyServiceTests.Proxy.AuthHeaderProvider.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class ClientsService : Interfaces.IClientsService
    {
        private readonly IntegrationServices.IClientsService _ntegrationServicesIClientsService;

        [IntentManaged(Mode.Merge)]
        public ClientsService(IntegrationServices.IClientsService ntegrationServicesIClientsService)
        {
            _ntegrationServicesIClientsService = ntegrationServicesIClientsService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreateClient(
            ClientsServices.ClientCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            var result = await _ntegrationServicesIClientsService.CreateClientAsync(new IntegrationServices.Contracts.OriginalServices.Services.Clients.ClientCreateDto
            {
                Name = dto.Name
            }, cancellationToken);
            return result;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ClientDto> FindClientById(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await _ntegrationServicesIClientsService.FindClientByIdAsync(id, cancellationToken);
            return result;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<ClientDto>> FindClients(CancellationToken cancellationToken = default)
        {
            var result = await _ntegrationServicesIClientsService.FindClientsAsync(cancellationToken);
            return result;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateClient(
            Guid id,
            ClientsServices.ClientUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            await _ntegrationServicesIClientsService.UpdateClientAsync(id, new IntegrationServices.Contracts.OriginalServices.Services.Clients.ClientUpdateDto
            {
                Id = dto.Id,
                Name = dto.Name
            }, cancellationToken);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteClient(Guid id, CancellationToken cancellationToken = default)
        {
            await _ntegrationServicesIClientsService.DeleteClientAsync(id, cancellationToken);
        }

        public void Dispose()
        {
        }
    }
}