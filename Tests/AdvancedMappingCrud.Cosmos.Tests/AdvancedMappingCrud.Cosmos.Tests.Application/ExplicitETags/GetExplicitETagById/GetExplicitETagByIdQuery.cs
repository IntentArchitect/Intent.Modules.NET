using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.ExplicitETags.GetExplicitETagById
{
    public class GetExplicitETagByIdQuery : IRequest<ExplicitETagDto>, IQuery
    {
        public GetExplicitETagByIdQuery(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }
}