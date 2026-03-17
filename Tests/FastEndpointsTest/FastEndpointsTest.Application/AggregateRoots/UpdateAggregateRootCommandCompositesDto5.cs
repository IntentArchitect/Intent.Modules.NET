using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public record UpdateAggregateRootCommandCompositesDto5
    {
        public UpdateAggregateRootCommandCompositesDto5()
        {
            CompositeAttr = null!;
        }

        public Guid Id { get; init; }
        public string CompositeAttr { get; init; }

        public static UpdateAggregateRootCommandCompositesDto5 Create(Guid id, string compositeAttr)
        {
            return new UpdateAggregateRootCommandCompositesDto5
            {
                Id = id,
                CompositeAttr = compositeAttr
            };
        }
    }
}