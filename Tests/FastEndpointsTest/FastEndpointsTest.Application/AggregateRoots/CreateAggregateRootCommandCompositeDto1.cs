using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public record CreateAggregateRootCommandCompositeDto1
    {
        public CreateAggregateRootCommandCompositeDto1()
        {
            CompositeAttr = null!;
            Composites = null!;
        }

        public string CompositeAttr { get; init; }
        public List<CreateAggregateRootCommandCompositesDto2> Composites { get; init; }
        public CreateAggregateRootCommandCompositeDto2? Composite { get; init; }

        public static CreateAggregateRootCommandCompositeDto1 Create(
            string compositeAttr,
            List<CreateAggregateRootCommandCompositesDto2> composites,
            CreateAggregateRootCommandCompositeDto2? composite)
        {
            return new CreateAggregateRootCommandCompositeDto1
            {
                CompositeAttr = compositeAttr,
                Composites = composites,
                Composite = composite
            };
        }
    }
}