using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ProxyServiceTests.Proxy.TMS.Application.Common.Interfaces;
using ProxyServiceTests.Proxy.TMS.Application.IntegrationServices.Contracts.OriginalServices.Services.Clients;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace ProxyServiceTests.Proxy.TMS.Application.ClientsServices.FindClients
{
    public class FindClientsQuery : IRequest<List<ClientDto>>, IQuery
    {
        public FindClientsQuery()
        {
        }
    }
}