using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots
{
    public class UpdateAggregateRootCompositeManyBCompositeSingleBBDto
    {
        public UpdateAggregateRootCompositeManyBCompositeSingleBBDto()
        {
            CompositeAttr = null!;
        }

        public string CompositeAttr { get; set; }
        public Guid Id { get; set; }

        public static UpdateAggregateRootCompositeManyBCompositeSingleBBDto Create(string compositeAttr, Guid id)
        {
            return new UpdateAggregateRootCompositeManyBCompositeSingleBBDto
            {
                CompositeAttr = compositeAttr,
                Id = id
            };
        }
    }
}