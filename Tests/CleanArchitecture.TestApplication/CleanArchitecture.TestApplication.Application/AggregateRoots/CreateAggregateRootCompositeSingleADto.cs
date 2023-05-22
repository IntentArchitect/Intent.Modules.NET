using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRoots
{

    public class CreateAggregateRootCompositeSingleADto
    {
        public CreateAggregateRootCompositeSingleADto()
        {
        }

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

        public string CompositeAttr { get; set; } = null!;

        public CreateAggregateRootCompositeSingleACompositeSingleAADto? Composite { get; set; }

        public List<CreateAggregateRootCompositeSingleACompositeManyAADto> Composites { get; set; } = null!;

    }
}