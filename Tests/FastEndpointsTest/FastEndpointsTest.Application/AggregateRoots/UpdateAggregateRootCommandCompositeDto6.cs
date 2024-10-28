using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public class UpdateAggregateRootCommandCompositeDto6
    {
        public UpdateAggregateRootCommandCompositeDto6()
        {
            CompositeAttr = null!;
            Composites = null!;
        }

        public Guid Id { get; set; }
        public string CompositeAttr { get; set; }
        public List<UpdateAggregateRootCommandCompositesDto5> Composites { get; set; }
        public UpdateAggregateRootCommandCompositeDto7? Composite { get; set; }

        public static UpdateAggregateRootCommandCompositeDto6 Create(
            Guid id,
            string compositeAttr,
            List<UpdateAggregateRootCommandCompositesDto5> composites,
            UpdateAggregateRootCommandCompositeDto7? composite)
        {
            return new UpdateAggregateRootCommandCompositeDto6
            {
                Id = id,
                CompositeAttr = compositeAttr,
                Composites = composites,
                Composite = composite
            };
        }
    }
}