using CleanArchitecture.Dapr.InvocationClient.Application.IntegrationServices;
using CleanArchitecture.Dapr.InvocationClient.Application.IntegrationServices.Contracts.Services.AdvancedMappingSystem.Clients;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Dapr.InvocationClient.Application.Clients.CallGetClient
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CallGetClientCommandHandler : IRequestHandler<CallGetClientCommand>
    {
        private readonly IClientsService _clientsService;

        [IntentManaged(Mode.Merge)]
        public CallGetClientCommandHandler(IClientsService clientsService)
        {
            _clientsService = clientsService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CallGetClientCommand request, CancellationToken cancellationToken)
        {
            var client = await _clientsService.GetClientByIdAsync(new GetClientByIdQuery
            {
                Id = request.Id
            }, cancellationToken);
        }
    }
}