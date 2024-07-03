using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ProxyServiceTests.Proxy.PTH.Application.IntegrationServices;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace ProxyServiceTests.Proxy.PTH.Application.DeleteClientsServices.DeleteClient
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteClientCommandHandler : IRequestHandler<DeleteClientCommand>
    {
        private readonly IDeleteClientsService _deleteClientsService;

        [IntentManaged(Mode.Merge)]
        public DeleteClientCommandHandler(IDeleteClientsService deleteClientsService)
        {
            _deleteClientsService = deleteClientsService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteClientCommand request, CancellationToken cancellationToken)
        {
            await _deleteClientsService.DeleteClientAsync(request.Id, cancellationToken);
        }
    }
}