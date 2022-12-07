using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRoots
{

    public class CreateAggregateRootCompositeManyBDTO
    {
        public CreateAggregateRootCompositeManyBDTO()
        {
        }

        public static CreateAggregateRootCompositeManyBDTO Create(
            string compositeAttr,
            DateTime? someDate,
            CreateAggregateRootCompositeManyBCompositeSingleBBDTO? composite,
            List<CreateAggregateRootCompositeManyBCompositeManyBBDTO> composites)
        {
            return new CreateAggregateRootCompositeManyBDTO
            {
                CompositeAttr = compositeAttr,
                SomeDate = someDate,
                Composite = composite,
                Composites = composites,
            };
        }

        public string CompositeAttr { get; set; }

        public DateTime? SomeDate { get; set; }

        public CreateAggregateRootCompositeManyBCompositeSingleBBDTO? Composite { get; set; }

        public List<CreateAggregateRootCompositeManyBCompositeManyBBDTO> Composites { get; set; }

    }
}