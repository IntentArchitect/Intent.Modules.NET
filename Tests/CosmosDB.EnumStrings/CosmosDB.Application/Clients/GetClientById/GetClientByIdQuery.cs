using CosmosDB.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CosmosDB.Application.Clients.GetClientById
{
    public class GetClientByIdQuery : IRequest<ClientDto>, IQuery
    {
        public GetClientByIdQuery(string identifier)
        {
            Identifier = identifier;
        }

        public string Identifier { get; set; }
    }
}