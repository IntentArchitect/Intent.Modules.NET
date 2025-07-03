using CleanArchitecture.Dapr.InvocationClient.Application.IntegrationServices;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Dapr.InvocationClient.Application.Clients.CallGetClients
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CallGetClientsCommandHandler : IRequestHandler<CallGetClientsCommand>
    {
        private readonly IClientsService _clientsService;

        [IntentManaged(Mode.Merge)]
        public CallGetClientsCommandHandler(IClientsService clientsService)
        {
            _clientsService = clientsService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CallGetClientsCommand request, CancellationToken cancellationToken)
        {
            var clients = await _clientsService.GetClientsAsync(cancellationToken);
        }
    }
}