using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Parents.GetParentById
{
    public class GetParentByIdQuery : IRequest<ParentDto>, IQuery
    {
        public GetParentByIdQuery(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }
}