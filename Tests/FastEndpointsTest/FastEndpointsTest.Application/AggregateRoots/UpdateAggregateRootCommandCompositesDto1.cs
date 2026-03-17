using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public record UpdateAggregateRootCommandCompositesDto1
    {
        public UpdateAggregateRootCommandCompositesDto1()
        {
            CompositeAttr = null!;
        }

        public Guid Id { get; init; }
        public string CompositeAttr { get; init; }

        public static UpdateAggregateRootCommandCompositesDto1 Create(Guid id, string compositeAttr)
        {
            return new UpdateAggregateRootCommandCompositesDto1
            {
                Id = id,
                CompositeAttr = compositeAttr
            };
        }
    }
}