using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRoots
{

    public class CreateAggregateRootCompositeSingleACompositeSingleAADto
    {
        public CreateAggregateRootCompositeSingleACompositeSingleAADto()
        {
            CompositeAttr = null!;
        }

        public static CreateAggregateRootCompositeSingleACompositeSingleAADto Create(string compositeAttr)
        {
            return new CreateAggregateRootCompositeSingleACompositeSingleAADto
            {
                CompositeAttr = compositeAttr
            };
        }

        public string CompositeAttr { get; set; }

    }
}