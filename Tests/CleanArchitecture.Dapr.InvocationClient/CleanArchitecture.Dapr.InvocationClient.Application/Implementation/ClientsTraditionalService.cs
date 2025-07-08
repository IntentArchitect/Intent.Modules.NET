using CleanArchitecture.Dapr.InvocationClient.Application.IntegrationServices;
using CleanArchitecture.Dapr.InvocationClient.Application.IntegrationServices.Contracts.Services.AdvancedMappingSystem.Clients;
using CleanArchitecture.Dapr.InvocationClient.Application.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace CleanArchitecture.Dapr.InvocationClient.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class ClientsTraditionalService : IClientsTraditionalService
    {
        private readonly IClientsService _clientsService;

        [IntentManaged(Mode.Merge)]
        public ClientsTraditionalService(IClientsService clientsService)
        {
            _clientsService = clientsService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task CallGetClientsQuery(CancellationToken cancellationToken = default)
        {
            var result = await _clientsService.GetClientsAsync(cancellationToken);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task CallGetClientByIdQuery(string id, CancellationToken cancellationToken = default)
        {
            var result = await _clientsService.GetClientByIdAsync(id, cancellationToken);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task CallCreateClientCommand(
            string name,
            List<string> tagsIds,
            CancellationToken cancellationToken = default)
        {
            var result = await _clientsService.CreateClientAsync(new CreateClientCommand
            {
                Name = name,
                TagsIds = tagsIds
            }, cancellationToken);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task CallDeleteClientCommand(string id, CancellationToken cancellationToken = default)
        {
            await _clientsService.DeleteClientAsync(id, cancellationToken);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task CallUpdateClientCommand(
            string id,
            string name,
            List<string> tagsIds,
            CancellationToken cancellationToken = default)
        {
            await _clientsService.UpdateClientAsync(new UpdateClientCommand
            {
                Id = id,
                Name = name,
                TagsIds = tagsIds
            }, cancellationToken);
        }
    }
}