using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRoots
{

    public class CreateAggregateRootCompositeManyBCompositeManyBBDTO
    {
        public CreateAggregateRootCompositeManyBCompositeManyBBDTO()
        {
        }

        public static CreateAggregateRootCompositeManyBCompositeManyBBDTO Create(
            string compositeAttr,
            Guid aCompositeManyId)
        {
            return new CreateAggregateRootCompositeManyBCompositeManyBBDTO
            {
                CompositeAttr = compositeAttr,
                ACompositeManyId = aCompositeManyId,
            };
        }

        public string CompositeAttr { get; set; }

        public Guid ACompositeManyId { get; set; }

    }
}