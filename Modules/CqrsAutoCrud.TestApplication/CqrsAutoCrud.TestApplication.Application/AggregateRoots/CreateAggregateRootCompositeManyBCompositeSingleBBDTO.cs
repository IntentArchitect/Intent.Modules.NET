using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRoots
{

    public class CreateAggregateRootCompositeManyBCompositeSingleBBDTO
    {
        public CreateAggregateRootCompositeManyBCompositeSingleBBDTO()
        {
        }

        public static CreateAggregateRootCompositeManyBCompositeSingleBBDTO Create(
            string compositeAttr)
        {
            return new CreateAggregateRootCompositeManyBCompositeSingleBBDTO
            {
                CompositeAttr = compositeAttr,
            };
        }

        public string CompositeAttr { get; set; }

    }
}