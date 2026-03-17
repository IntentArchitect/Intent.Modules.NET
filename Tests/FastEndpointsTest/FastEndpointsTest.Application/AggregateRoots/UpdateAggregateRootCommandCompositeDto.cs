using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public record UpdateAggregateRootCommandCompositeDto
    {
        public UpdateAggregateRootCommandCompositeDto()
        {
            CompositeAttr = null!;
        }

        public Guid Id { get; init; }
        public string CompositeAttr { get; init; }

        public static UpdateAggregateRootCommandCompositeDto Create(Guid id, string compositeAttr)
        {
            return new UpdateAggregateRootCommandCompositeDto
            {
                Id = id,
                CompositeAttr = compositeAttr
            };
        }
    }
}