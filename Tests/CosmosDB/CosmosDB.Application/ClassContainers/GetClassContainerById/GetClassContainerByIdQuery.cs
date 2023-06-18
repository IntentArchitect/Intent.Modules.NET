using CosmosDB.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CosmosDB.Application.ClassContainers.GetClassContainerById
{
    public class GetClassContainerByIdQuery : IRequest<ClassContainerDto>, IQuery
    {
        public GetClassContainerByIdQuery(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }
}