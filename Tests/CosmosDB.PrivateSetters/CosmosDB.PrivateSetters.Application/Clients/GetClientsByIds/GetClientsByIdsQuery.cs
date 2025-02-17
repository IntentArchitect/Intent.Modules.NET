using System.Collections.Generic;
using CosmosDB.PrivateSetters.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Application.Clients.GetClientsByIds
{
    public class GetClientsByIdsQuery : IRequest<List<ClientDto>>, IQuery
    {
        public GetClientsByIdsQuery(List<string> ids)
        {
            Ids = ids;
        }

        public List<string> Ids { get; set; }
    }
}