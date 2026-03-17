using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public record CreateAggregateRootCommandCompositesDto
    {
        public CreateAggregateRootCommandCompositesDto()
        {
            CompositeAttr = null!;
            Composites = null!;
        }

        public string CompositeAttr { get; init; }
        public DateTime? SomeDate { get; init; }
        public List<CreateAggregateRootCommandCompositesDto1> Composites { get; init; }
        public CreateAggregateRootCommandCompositeDto? Composite { get; init; }

        public static CreateAggregateRootCommandCompositesDto Create(
            string compositeAttr,
            DateTime? someDate,
            List<CreateAggregateRootCommandCompositesDto1> composites,
            CreateAggregateRootCommandCompositeDto? composite)
        {
            return new CreateAggregateRootCommandCompositesDto
            {
                CompositeAttr = compositeAttr,
                SomeDate = someDate,
                Composites = composites,
                Composite = composite
            };
        }
    }
}