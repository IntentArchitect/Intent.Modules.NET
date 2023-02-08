using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRoots
{

    public class UpdateAggregateRootCompositeManyBCompositeManyBBDto
    {
        public UpdateAggregateRootCompositeManyBCompositeManyBBDto()
        {
        }

        public static UpdateAggregateRootCompositeManyBCompositeManyBBDto Create(
            string compositeAttr,
            Guid compositeManyBId,
            Guid id,
            string compositeAttr,
            Guid compositeManyBId,
            Guid id)
        {
            return new UpdateAggregateRootCompositeManyBCompositeManyBBDto
            {
                CompositeAttr = compositeAttr,
                CompositeManyBId = compositeManyBId,
                Id = id,
                CompositeAttr = compositeAttr,
                CompositeManyBId = compositeManyBId,
                Id = id,
            };
        }

        public string CompositeAttr { get; set; }

        public Guid CompositeManyBId { get; set; }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public Guid CompositeManyBId { get; set; }

        public Guid Id { get; set; }

    }
}