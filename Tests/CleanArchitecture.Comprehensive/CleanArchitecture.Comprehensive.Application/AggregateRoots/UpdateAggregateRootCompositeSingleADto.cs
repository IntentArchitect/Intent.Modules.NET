using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots
{
    public class UpdateAggregateRootCompositeSingleADto
    {
        public UpdateAggregateRootCompositeSingleADto()
        {
            CompositeAttr = null!;
            Composites = null!;
        }

        public string CompositeAttr { get; set; }
        public Guid Id { get; set; }
        public UpdateAggregateRootCompositeSingleACompositeSingleAADto? Composite { get; set; }
        public List<UpdateAggregateRootCompositeSingleACompositeManyAADto> Composites { get; set; }

        public static UpdateAggregateRootCompositeSingleADto Create(
            string compositeAttr,
            Guid id,
            UpdateAggregateRootCompositeSingleACompositeSingleAADto? composite,
            List<UpdateAggregateRootCompositeSingleACompositeManyAADto> composites)
        {
            return new UpdateAggregateRootCompositeSingleADto
            {
                CompositeAttr = compositeAttr,
                Id = id,
                Composite = composite,
                Composites = composites
            };
        }
    }
}