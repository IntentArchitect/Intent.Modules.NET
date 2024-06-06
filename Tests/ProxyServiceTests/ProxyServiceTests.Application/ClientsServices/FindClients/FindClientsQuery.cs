using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ProxyServiceTests.Application.Common.Interfaces;
using ProxyServiceTests.Application.IntegrationServices.Contracts.OriginalServices.Services.Clients;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace ProxyServiceTests.Application.ClientsServices.FindClients
{
    public class FindClientsQuery : IRequest<List<ClientDto>>, IQuery
    {
        public FindClientsQuery()
        {
        }
    }
}