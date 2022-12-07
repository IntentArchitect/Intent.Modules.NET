using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRoots
{

    public class UpdateAggregateRootCompositeManyBCompositeSingleBBDTO
    {
        public UpdateAggregateRootCompositeManyBCompositeSingleBBDTO()
        {
        }

        public static UpdateAggregateRootCompositeManyBCompositeSingleBBDTO Create(
            Guid id,
            string compositeAttr)
        {
            return new UpdateAggregateRootCompositeManyBCompositeSingleBBDTO
            {
                Id = id,
                CompositeAttr = compositeAttr,
            };
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

    }
}