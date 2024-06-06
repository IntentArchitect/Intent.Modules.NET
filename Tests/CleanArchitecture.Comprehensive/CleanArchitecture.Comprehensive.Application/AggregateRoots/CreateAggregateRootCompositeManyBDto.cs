using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots
{
    public class CreateAggregateRootCompositeManyBDto
    {
        public CreateAggregateRootCompositeManyBDto()
        {
            CompositeAttr = null!;
            Composites = null!;
        }

        public string CompositeAttr { get; set; }
        public DateTime? SomeDate { get; set; }
        public CreateAggregateRootCompositeManyBCompositeSingleBBDto? Composite { get; set; }
        public List<CreateAggregateRootCompositeManyBCompositeManyBBDto> Composites { get; set; }

        public static CreateAggregateRootCompositeManyBDto Create(
            string compositeAttr,
            DateTime? someDate,
            CreateAggregateRootCompositeManyBCompositeSingleBBDto? composite,
            List<CreateAggregateRootCompositeManyBCompositeManyBBDto> composites)
        {
            return new CreateAggregateRootCompositeManyBDto
            {
                CompositeAttr = compositeAttr,
                SomeDate = someDate,
                Composite = composite,
                Composites = composites
            };
        }
    }
}