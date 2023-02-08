using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRoots
{

    public class UpdateAggregateRootCompositeManyBDto
    {
        public UpdateAggregateRootCompositeManyBDto()
        {
        }

        public static UpdateAggregateRootCompositeManyBDto Create(
            string compositeAttr,
            DateTime? someDate,
            Guid aggregateRootId,
            Guid id,
            UpdateAggregateRootCompositeManyBCompositeSingleBBDto? composite,
            List<UpdateAggregateRootCompositeManyBCompositeManyBBDto> composites)
        {
            return new UpdateAggregateRootCompositeManyBDto
            {
                CompositeAttr = compositeAttr,
                SomeDate = someDate,
                AggregateRootId = aggregateRootId,
                Id = id,
                Composite = composite,
                Composites = composites,
            };
        }

        public string CompositeAttr { get; set; }

        public DateTime? SomeDate { get; set; }

        public Guid AggregateRootId { get; set; }

        public Guid Id { get; set; }

        public UpdateAggregateRootCompositeManyBCompositeSingleBBDto? Composite { get; set; }

        public List<UpdateAggregateRootCompositeManyBCompositeManyBBDto> Composites { get; set; }

    }
}