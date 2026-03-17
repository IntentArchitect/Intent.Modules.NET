using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public record UpdateAggregateRootCommandCompositeDto6
    {
        public UpdateAggregateRootCommandCompositeDto6()
        {
            CompositeAttr = null!;
            Composites = null!;
        }

        public Guid Id { get; init; }
        public string CompositeAttr { get; init; }
        public List<UpdateAggregateRootCommandCompositesDto5> Composites { get; init; }
        public UpdateAggregateRootCommandCompositeDto7? Composite { get; init; }

        public static UpdateAggregateRootCommandCompositeDto6 Create(
            Guid id,
            string compositeAttr,
            List<UpdateAggregateRootCommandCompositesDto5> composites,
            UpdateAggregateRootCommandCompositeDto7? composite)
        {
            return new UpdateAggregateRootCommandCompositeDto6
            {
                Id = id,
                CompositeAttr = compositeAttr,
                Composites = composites,
                Composite = composite
            };
        }
    }
}