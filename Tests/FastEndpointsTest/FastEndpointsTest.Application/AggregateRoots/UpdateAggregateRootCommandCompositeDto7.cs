using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public record UpdateAggregateRootCommandCompositeDto7
    {
        public UpdateAggregateRootCommandCompositeDto7()
        {
            CompositeAttr = null!;
        }

        public Guid Id { get; init; }
        public string CompositeAttr { get; init; }

        public static UpdateAggregateRootCommandCompositeDto7 Create(Guid id, string compositeAttr)
        {
            return new UpdateAggregateRootCommandCompositeDto7
            {
                Id = id,
                CompositeAttr = compositeAttr
            };
        }
    }
}