using Intent.RoslynWeaver.Attributes;
using MediatR;
using Redis.Om.Repositories.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Redis.Om.Repositories.Application.Clients.GetClientById
{
    public class GetClientByIdQuery : IRequest<ClientDto>, IQuery
    {
        public GetClientByIdQuery(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }
}