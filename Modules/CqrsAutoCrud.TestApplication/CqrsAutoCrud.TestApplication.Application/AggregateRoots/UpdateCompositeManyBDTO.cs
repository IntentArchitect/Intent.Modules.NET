using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRoots
{

    public class UpdateCompositeManyBDTO
    {
        public UpdateCompositeManyBDTO()
        {
        }

        public static UpdateCompositeManyBDTO Create(
            Guid id,
            string compositeAttr,
            Guid aggregateRootId,
            UpdateCompositeSingleBBDTO? composite,
            List<UpdateCompositeManyBBDTO> composites)
        {
            return new UpdateCompositeManyBDTO
            {
                Id = id,
                CompositeAttr = compositeAttr,
                AggregateRootId = aggregateRootId,
                Composite = composite,
                Composites = composites,
            };
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public Guid AggregateRootId { get; set; }

        public UpdateCompositeSingleBBDTO? Composite { get; set; }

        public List<UpdateCompositeManyBBDTO> Composites { get; set; }

    }
}