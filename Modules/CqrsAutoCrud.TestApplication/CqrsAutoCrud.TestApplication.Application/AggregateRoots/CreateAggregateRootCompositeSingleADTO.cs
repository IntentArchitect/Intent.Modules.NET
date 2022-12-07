using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRoots
{

    public class CreateAggregateRootCompositeSingleADTO
    {
        public CreateAggregateRootCompositeSingleADTO()
        {
        }

        public static CreateAggregateRootCompositeSingleADTO Create(
            string compositeAttr,
            CreateAggregateRootCompositeSingleACompositeSingleAADTO? composite,
            List<CreateAggregateRootCompositeSingleACompositeManyAADTO> composites)
        {
            return new CreateAggregateRootCompositeSingleADTO
            {
                CompositeAttr = compositeAttr,
                Composite = composite,
                Composites = composites,
            };
        }

        public string CompositeAttr { get; set; }

        public CreateAggregateRootCompositeSingleACompositeSingleAADTO? Composite { get; set; }

        public List<CreateAggregateRootCompositeSingleACompositeManyAADTO> Composites { get; set; }

    }
}