using CleanArchitecture.Dapr.InvocationClient.Application.IntegrationServices;
using CleanArchitecture.Dapr.InvocationClient.Application.IntegrationServices.Contracts.Services.AdvancedMappingSystem.Clients;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Dapr.InvocationClient.Application.Clients.CallUpdateClient
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CallUpdateClientCommandHandler : IRequestHandler<CallUpdateClientCommand>
    {
        private readonly IClientsService _clientsService;

        [IntentManaged(Mode.Merge)]
        public CallUpdateClientCommandHandler(IClientsService clientsService)
        {
            _clientsService = clientsService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CallUpdateClientCommand request, CancellationToken cancellationToken)
        {
            await _clientsService.UpdateClientAsync(new UpdateClientCommand
            {
                Id = request.Id,
                Name = request.Name,
                TagsIds = request.TagsIds
            }, cancellationToken);
        }
    }
}