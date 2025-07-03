using CleanArchitecture.Dapr.InvocationClient.Application.IntegrationServices;
using CleanArchitecture.Dapr.InvocationClient.Application.IntegrationServices.Contracts.Services.AdvancedMappingSystem.Clients;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Dapr.InvocationClient.Application.Clients.CallCreateClient
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CallCreateClientCommandHandler : IRequestHandler<CallCreateClientCommand>
    {
        private readonly IClientsService _clientsService;

        [IntentManaged(Mode.Merge)]
        public CallCreateClientCommandHandler(IClientsService clientsService)
        {
            _clientsService = clientsService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CallCreateClientCommand request, CancellationToken cancellationToken)
        {
            var clientId = await _clientsService.CreateClientAsync(new CreateClientCommand
            {
                Name = request.Name,
                TagsIds = request.TagsIds
            }, cancellationToken);
        }
    }
}