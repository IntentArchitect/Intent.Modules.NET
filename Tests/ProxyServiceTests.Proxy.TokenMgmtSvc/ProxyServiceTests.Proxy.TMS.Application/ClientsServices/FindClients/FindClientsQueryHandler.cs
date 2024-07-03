using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ProxyServiceTests.Proxy.TMS.Application.IntegrationServices;
using ProxyServiceTests.Proxy.TMS.Application.IntegrationServices.Contracts.OriginalServices.Services.Clients;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace ProxyServiceTests.Proxy.TMS.Application.ClientsServices.FindClients
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class FindClientsQueryHandler : IRequestHandler<FindClientsQuery, List<ClientDto>>
    {
        private readonly IClientsService _clientsService;
        [IntentManaged(Mode.Merge)]
        public FindClientsQueryHandler(IClientsService clientsService)
        {
            _clientsService = clientsService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<ClientDto>> Handle(FindClientsQuery request, CancellationToken cancellationToken)
        {
            var result = await _clientsService.FindClientsAsync(cancellationToken);
            return result;
        }
    }
}