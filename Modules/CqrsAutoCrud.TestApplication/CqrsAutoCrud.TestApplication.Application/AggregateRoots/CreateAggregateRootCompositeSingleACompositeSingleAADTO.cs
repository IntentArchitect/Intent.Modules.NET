using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRoots
{

    public class CreateAggregateRootCompositeSingleACompositeSingleAADTO
    {
        public CreateAggregateRootCompositeSingleACompositeSingleAADTO()
        {
        }

        public static CreateAggregateRootCompositeSingleACompositeSingleAADTO Create(
            string compositeAttr)
        {
            return new CreateAggregateRootCompositeSingleACompositeSingleAADTO
            {
                CompositeAttr = compositeAttr,
            };
        }

        public string CompositeAttr { get; set; }

    }
}