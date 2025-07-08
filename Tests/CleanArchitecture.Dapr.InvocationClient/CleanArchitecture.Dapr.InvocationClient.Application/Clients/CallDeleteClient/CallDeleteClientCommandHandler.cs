using CleanArchitecture.Dapr.InvocationClient.Application.IntegrationServices;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Dapr.InvocationClient.Application.Clients.CallDeleteClient
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CallDeleteClientCommandHandler : IRequestHandler<CallDeleteClientCommand>
    {
        private readonly IClientsService _clientsService;

        [IntentManaged(Mode.Merge)]
        public CallDeleteClientCommandHandler(IClientsService clientsService)
        {
            _clientsService = clientsService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CallDeleteClientCommand request, CancellationToken cancellationToken)
        {
            await _clientsService.DeleteClientAsync(request.Id, cancellationToken);
        }
    }
}