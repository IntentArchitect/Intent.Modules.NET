using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ProxyServiceTests.Application.IntegrationServices;
using ProxyServiceTests.Application.IntegrationServices.Contracts.OriginalServices.Services.Clients;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace ProxyServiceTests.Application.ClientsServices.CreateClient
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, Guid>
    {
        private readonly IClientsService _clientsService;
        [IntentManaged(Mode.Merge)]
        public CreateClientCommandHandler(IClientsService clientsService)
        {
            _clientsService = clientsService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateClientCommand request, CancellationToken cancellationToken)
        {
            var result = await _clientsService.CreateClientAsync(new ClientCreateDto
            {
                Name = request.Name
            }, cancellationToken);
            return result;
        }
    }
}