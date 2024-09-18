using System.Collections.Generic;
using CosmosDB.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CosmosDB.Application.Clients.GetClientByName
{
    public class GetClientByNameQuery : IRequest<List<ClientDto>>, IQuery
    {
        public GetClientByNameQuery(string searchText)
        {
            SearchText = searchText;
        }

        public string SearchText { get; set; }
    }
}