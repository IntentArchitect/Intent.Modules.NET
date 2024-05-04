using CleanArchitecture.Dapr.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Tags.GetTagById
{
    public class GetTagByIdQuery : IRequest<TagDto>, IQuery
    {
        public GetTagByIdQuery(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }
}