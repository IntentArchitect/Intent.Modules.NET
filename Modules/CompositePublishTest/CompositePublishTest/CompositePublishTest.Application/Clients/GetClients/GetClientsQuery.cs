using System.Collections.Generic;
using CompositePublishTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CompositePublishTest.Application.Clients.GetClients
{
    public class GetClientsQuery : IRequest<List<ClientDto>>, IQuery
    {
        public GetClientsQuery()
        {
        }
    }
}