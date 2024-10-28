using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public class UpdateAggregateRootCommandCompositeDto1
    {
        public UpdateAggregateRootCommandCompositeDto1()
        {
            CompositeAttr = null!;
            Composites = null!;
        }

        public Guid Id { get; set; }
        public string CompositeAttr { get; set; }
        public List<UpdateAggregateRootCommandCompositesDto2> Composites { get; set; }
        public UpdateAggregateRootCommandCompositeDto2? Composite { get; set; }

        public static UpdateAggregateRootCommandCompositeDto1 Create(
            Guid id,
            string compositeAttr,
            List<UpdateAggregateRootCommandCompositesDto2> composites,
            UpdateAggregateRootCommandCompositeDto2? composite)
        {
            return new UpdateAggregateRootCommandCompositeDto1
            {
                Id = id,
                CompositeAttr = compositeAttr,
                Composites = composites,
                Composite = composite
            };
        }
    }
}