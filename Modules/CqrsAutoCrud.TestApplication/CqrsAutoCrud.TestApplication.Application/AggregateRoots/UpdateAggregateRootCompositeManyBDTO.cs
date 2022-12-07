using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRoots
{

    public class UpdateAggregateRootCompositeManyBDTO
    {
        public UpdateAggregateRootCompositeManyBDTO()
        {
        }

        public static UpdateAggregateRootCompositeManyBDTO Create(
            Guid id,
            string compositeAttr,
            Guid aggregateRootId,
            DateTime? someDate,
            UpdateAggregateRootCompositeManyBCompositeSingleBBDTO? composite,
            List<UpdateAggregateRootCompositeManyBCompositeManyBBDTO> composites)
        {
            return new UpdateAggregateRootCompositeManyBDTO
            {
                Id = id,
                CompositeAttr = compositeAttr,
                AggregateRootId = aggregateRootId,
                SomeDate = someDate,
                Composite = composite,
                Composites = composites,
            };
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public Guid AggregateRootId { get; set; }

        public DateTime? SomeDate { get; set; }

        public UpdateAggregateRootCompositeManyBCompositeSingleBBDTO? Composite { get; set; }

        public List<UpdateAggregateRootCompositeManyBCompositeManyBBDTO> Composites { get; set; }

    }
}