using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots
{
    public class CreateAggregateRootCompositeSingleADto
    {
        public CreateAggregateRootCompositeSingleADto()
        {
            CompositeAttr = null!;
            Composites = null!;
        }

        public string CompositeAttr { get; set; }
        public CreateAggregateRootCompositeSingleACompositeSingleAADto? Composite { get; set; }
        public List<CreateAggregateRootCompositeSingleACompositeManyAADto> Composites { get; set; }

        public static CreateAggregateRootCompositeSingleADto Create(
            string compositeAttr,
            CreateAggregateRootCompositeSingleACompositeSingleAADto? composite,
            List<CreateAggregateRootCompositeSingleACompositeManyAADto> composites)
        {
            return new CreateAggregateRootCompositeSingleADto
            {
                CompositeAttr = compositeAttr,
                Composite = composite,
                Composites = composites
            };
        }
    }
}