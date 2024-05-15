using CleanArchitecture.Dapr.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.AdvancedMappingSystem.Clients.GetClientById
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