using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public record UpdateAggregateRootCommandCompositesDto
    {
        public UpdateAggregateRootCommandCompositesDto()
        {
            CompositeAttr = null!;
            Composites = null!;
        }

        public Guid Id { get; init; }
        public string CompositeAttr { get; init; }
        public DateTime? SomeDate { get; init; }
        public List<UpdateAggregateRootCommandCompositesDto1> Composites { get; init; }
        public UpdateAggregateRootCommandCompositeDto? Composite { get; init; }

        public static UpdateAggregateRootCommandCompositesDto Create(
            Guid id,
            string compositeAttr,
            DateTime? someDate,
            List<UpdateAggregateRootCommandCompositesDto1> composites,
            UpdateAggregateRootCommandCompositeDto? composite)
        {
            return new UpdateAggregateRootCommandCompositesDto
            {
                Id = id,
                CompositeAttr = compositeAttr,
                SomeDate = someDate,
                Composites = composites,
                Composite = composite
            };
        }
    }
}