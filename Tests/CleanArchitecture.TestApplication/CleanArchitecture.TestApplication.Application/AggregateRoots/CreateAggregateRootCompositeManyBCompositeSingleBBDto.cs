using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRoots
{

    public class CreateAggregateRootCompositeManyBCompositeSingleBBDto
    {
        public CreateAggregateRootCompositeManyBCompositeSingleBBDto()
        {
        }

        public static CreateAggregateRootCompositeManyBCompositeSingleBBDto Create(
            string compositeAttr,
            string compositeAttr)
        {
            return new CreateAggregateRootCompositeManyBCompositeSingleBBDto
            {
                CompositeAttr = compositeAttr,
                CompositeAttr = compositeAttr,
            };
        }

        public string CompositeAttr { get; set; }

        public string CompositeAttr { get; set; }

    }
}