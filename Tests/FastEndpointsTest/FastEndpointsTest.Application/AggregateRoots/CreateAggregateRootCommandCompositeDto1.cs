using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public class CreateAggregateRootCommandCompositeDto1
    {
        public CreateAggregateRootCommandCompositeDto1()
        {
            CompositeAttr = null!;
            Composites = null!;
        }

        public string CompositeAttr { get; set; }
        public List<CreateAggregateRootCommandCompositesDto2> Composites { get; set; }
        public CreateAggregateRootCommandCompositeDto2? Composite { get; set; }

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