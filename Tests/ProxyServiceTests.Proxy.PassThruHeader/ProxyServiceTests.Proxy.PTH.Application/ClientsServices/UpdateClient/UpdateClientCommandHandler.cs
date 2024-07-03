using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ProxyServiceTests.Proxy.PTH.Application.IntegrationServices;
using ProxyServiceTests.Proxy.PTH.Application.IntegrationServices.Contracts.OriginalServices.Services.Clients;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace ProxyServiceTests.Proxy.PTH.Application.ClientsServices.UpdateClient
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateClientCommandHandler : IRequestHandler<UpdateClientCommand>
    {
        private readonly IClientsService _clientsService;

        [IntentManaged(Mode.Merge)]
        public UpdateClientCommandHandler(IClientsService clientsService)
        {
            _clientsService = clientsService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateClientCommand request, CancellationToken cancellationToken)
        {
            await _clientsService.UpdateClientAsync(request.Id, new ClientUpdateDto
            {
                Id = request.ClientUpdateDtoId,
                Name = request.Name
            }, cancellationToken);
        }
    }
}