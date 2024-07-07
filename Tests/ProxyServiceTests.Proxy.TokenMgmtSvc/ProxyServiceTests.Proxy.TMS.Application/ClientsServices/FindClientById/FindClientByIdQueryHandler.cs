using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ProxyServiceTests.Proxy.TMS.Application.IntegrationServices;
using ProxyServiceTests.Proxy.TMS.Application.IntegrationServices.Contracts.OriginalServices.Services.Clients;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace ProxyServiceTests.Proxy.TMS.Application.ClientsServices.FindClientById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class FindClientByIdQueryHandler : IRequestHandler<FindClientByIdQuery, ClientDto>
    {
        private readonly IClientsService _clientsService;
        [IntentManaged(Mode.Merge)]
        public FindClientByIdQueryHandler(IClientsService clientsService)
        {
            _clientsService = clientsService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ClientDto> Handle(FindClientByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _clientsService.FindClientByIdAsync(request.Id, cancellationToken);
            return result;
        }
    }
}