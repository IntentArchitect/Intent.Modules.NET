using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public record UpdateAggregateRootCommandCompositeDto1
    {
        public UpdateAggregateRootCommandCompositeDto1()
        {
            CompositeAttr = null!;
            Composites = null!;
        }

        public Guid Id { get; init; }
        public string CompositeAttr { get; init; }
        public List<UpdateAggregateRootCommandCompositesDto2> Composites { get; init; }
        public UpdateAggregateRootCommandCompositeDto2? Composite { get; init; }

        public static UpdateAggregateRootCommandCompositeDto1 Create(
            Guid id,
            string compositeAttr,
            List<UpdateAggregateRootCommandCompositesDto2> composites,
            UpdateAggregateRootCommandCompositeDto2? composite)
        {
            return new UpdateAggregateRootCommandCompositeDto1
            {
                Id = id,
                CompositeAttr = compositeAttr,
                Composites = composites,
                Composite = composite
            };
        }
    }
}