using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public class CreateAggregateRootCommandCompositeDto4
    {
        public CreateAggregateRootCommandCompositeDto4()
        {
            CompositeAttr = null!;
            Composites = null!;
        }

        public string CompositeAttr { get; set; }
        public List<CreateAggregateRootCommandCompositesDto5> Composites { get; set; }
        public CreateAggregateRootCommandCompositeDto5? Composite { get; set; }

        public static CreateAggregateRootCommandCompositeDto4 Create(
            string compositeAttr,
            List<CreateAggregateRootCommandCompositesDto5> composites,
            CreateAggregateRootCommandCompositeDto5? composite)
        {
            return new CreateAggregateRootCommandCompositeDto4
            {
                CompositeAttr = compositeAttr,
                Composites = composites,
                Composite = composite
            };
        }
    }
}