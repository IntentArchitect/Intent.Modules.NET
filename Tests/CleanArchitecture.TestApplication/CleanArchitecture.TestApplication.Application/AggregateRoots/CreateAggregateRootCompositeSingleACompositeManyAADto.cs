using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRoots
{

    public class CreateAggregateRootCompositeSingleACompositeManyAADto
    {
        public CreateAggregateRootCompositeSingleACompositeManyAADto()
        {
            CompositeAttr = null!;
        }

        public static CreateAggregateRootCompositeSingleACompositeManyAADto Create(string compositeAttr)
        {
            return new CreateAggregateRootCompositeSingleACompositeManyAADto
            {
                CompositeAttr = compositeAttr
            };
        }

        public string CompositeAttr { get; set; }

    }
}