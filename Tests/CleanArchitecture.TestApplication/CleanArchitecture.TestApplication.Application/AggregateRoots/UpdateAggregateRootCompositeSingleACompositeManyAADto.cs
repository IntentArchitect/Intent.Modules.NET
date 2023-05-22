using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRoots
{

    public class UpdateAggregateRootCompositeSingleACompositeManyAADto
    {
        public UpdateAggregateRootCompositeSingleACompositeManyAADto()
        {
        }

        public static UpdateAggregateRootCompositeSingleACompositeManyAADto Create(
            string compositeAttr,
            Guid compositeSingleAId,
            Guid id)
        {
            return new UpdateAggregateRootCompositeSingleACompositeManyAADto
            {
                CompositeAttr = compositeAttr,
                CompositeSingleAId = compositeSingleAId,
                Id = id
            };
        }

        public string CompositeAttr { get; set; } = null!;

        public Guid CompositeSingleAId { get; set; }

        public Guid Id { get; set; }

    }
}