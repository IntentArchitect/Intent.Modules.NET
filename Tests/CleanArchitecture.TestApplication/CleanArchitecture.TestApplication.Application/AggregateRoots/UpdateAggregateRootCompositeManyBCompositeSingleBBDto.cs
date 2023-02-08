using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRoots
{

    public class UpdateAggregateRootCompositeManyBCompositeSingleBBDto
    {
        public UpdateAggregateRootCompositeManyBCompositeSingleBBDto()
        {
        }

        public static UpdateAggregateRootCompositeManyBCompositeSingleBBDto Create(
            string compositeAttr,
            Guid id,
            string compositeAttr,
            Guid id)
        {
            return new UpdateAggregateRootCompositeManyBCompositeSingleBBDto
            {
                CompositeAttr = compositeAttr,
                Id = id,
                CompositeAttr = compositeAttr,
                Id = id,
            };
        }

        public string CompositeAttr { get; set; }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public Guid Id { get; set; }

    }
}