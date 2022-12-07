using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRoots
{

    public class UpdateAggregateRootCompositeSingleACompositeManyAADTO
    {
        public UpdateAggregateRootCompositeSingleACompositeManyAADTO()
        {
        }

        public static UpdateAggregateRootCompositeSingleACompositeManyAADTO Create(
            Guid id,
            string compositeAttr,
            Guid aCompositeSingleId)
        {
            return new UpdateAggregateRootCompositeSingleACompositeManyAADTO
            {
                Id = id,
                CompositeAttr = compositeAttr,
                ACompositeSingleId = aCompositeSingleId,
            };
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public Guid ACompositeSingleId { get; set; }

    }
}