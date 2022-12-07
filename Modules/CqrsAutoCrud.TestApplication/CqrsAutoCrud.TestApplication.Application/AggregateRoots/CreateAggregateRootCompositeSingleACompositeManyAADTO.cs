using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRoots
{

    public class CreateAggregateRootCompositeSingleACompositeManyAADTO
    {
        public CreateAggregateRootCompositeSingleACompositeManyAADTO()
        {
        }

        public static CreateAggregateRootCompositeSingleACompositeManyAADTO Create(
            string compositeAttr,
            Guid aCompositeSingleId)
        {
            return new CreateAggregateRootCompositeSingleACompositeManyAADTO
            {
                CompositeAttr = compositeAttr,
                ACompositeSingleId = aCompositeSingleId,
            };
        }

        public string CompositeAttr { get; set; }

        public Guid ACompositeSingleId { get; set; }

    }
}