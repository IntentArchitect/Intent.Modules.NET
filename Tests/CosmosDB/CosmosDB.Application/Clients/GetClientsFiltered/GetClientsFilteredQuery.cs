using System.Collections.Generic;
using CosmosDB.Application.Common.Interfaces;
using CosmosDB.Domain;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CosmosDB.Application.Clients.GetClientsFiltered
{
    public class GetClientsFilteredQuery : IRequest<List<ClientDto>>, IQuery
    {
        public GetClientsFilteredQuery(ClientType? type, string? name)
        {
            Type = type;
            Name = name;
        }

        public ClientType? Type { get; set; }
        public string? Name { get; set; }
    }
}