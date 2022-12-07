using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRoots
{

    public class UpdateAggregateRootCompositeSingleADTO
    {
        public UpdateAggregateRootCompositeSingleADTO()
        {
        }

        public static UpdateAggregateRootCompositeSingleADTO Create(
            Guid id,
            string compositeAttr,
            UpdateAggregateRootCompositeSingleACompositeSingleAADTO? composite,
            List<UpdateAggregateRootCompositeSingleACompositeManyAADTO> composites)
        {
            return new UpdateAggregateRootCompositeSingleADTO
            {
                Id = id,
                CompositeAttr = compositeAttr,
                Composite = composite,
                Composites = composites,
            };
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public UpdateAggregateRootCompositeSingleACompositeSingleAADTO? Composite { get; set; }

        public List<UpdateAggregateRootCompositeSingleACompositeManyAADTO> Composites { get; set; }

    }
}