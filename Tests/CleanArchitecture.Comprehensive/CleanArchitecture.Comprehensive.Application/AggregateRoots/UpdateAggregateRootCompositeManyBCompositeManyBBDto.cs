using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots
{
    public class UpdateAggregateRootCompositeManyBCompositeManyBBDto
    {
        public UpdateAggregateRootCompositeManyBCompositeManyBBDto()
        {
            CompositeAttr = null!;
        }

        public string CompositeAttr { get; set; }
        public Guid CompositeManyBId { get; set; }
        public Guid Id { get; set; }

        public static UpdateAggregateRootCompositeManyBCompositeManyBBDto Create(
            string compositeAttr,
            Guid compositeManyBId,
            Guid id)
        {
            return new UpdateAggregateRootCompositeManyBCompositeManyBBDto
            {
                CompositeAttr = compositeAttr,
                CompositeManyBId = compositeManyBId,
                Id = id
            };
        }
    }
}