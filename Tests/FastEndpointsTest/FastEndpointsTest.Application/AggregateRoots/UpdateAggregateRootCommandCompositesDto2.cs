using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public record UpdateAggregateRootCommandCompositesDto2
    {
        public UpdateAggregateRootCommandCompositesDto2()
        {
            CompositeAttr = null!;
        }

        public Guid Id { get; init; }
        public string CompositeAttr { get; init; }

        public static UpdateAggregateRootCommandCompositesDto2 Create(Guid id, string compositeAttr)
        {
            return new UpdateAggregateRootCommandCompositesDto2
            {
                Id = id,
                CompositeAttr = compositeAttr
            };
        }
    }
}