using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ProxyServiceTests.Proxy.TMS.Application.IntegrationServices;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace ProxyServiceTests.Proxy.TMS.Application.ClientsServices.DeleteClient
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteClientCommandHandler : IRequestHandler<DeleteClientCommand>
    {
        private readonly IClientsService _clientsService;
        [IntentManaged(Mode.Merge)]
        public DeleteClientCommandHandler(IClientsService clientsService)
        {
            _clientsService = clientsService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteClientCommand request, CancellationToken cancellationToken)
        {
            await _clientsService.DeleteClientAsync(request.Id, cancellationToken);
        }
    }
}