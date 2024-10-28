using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public class UpdateAggregateRootCommandCompositesDto
    {
        public UpdateAggregateRootCommandCompositesDto()
        {
            CompositeAttr = null!;
            Composites = null!;
        }

        public Guid Id { get; set; }
        public string CompositeAttr { get; set; }
        public DateTime? SomeDate { get; set; }
        public List<UpdateAggregateRootCommandCompositesDto1> Composites { get; set; }
        public UpdateAggregateRootCommandCompositeDto? Composite { get; set; }

        public static UpdateAggregateRootCommandCompositesDto Create(
            Guid id,
            string compositeAttr,
            DateTime? someDate,
            List<UpdateAggregateRootCommandCompositesDto1> composites,
            UpdateAggregateRootCommandCompositeDto? composite)
        {
            return new UpdateAggregateRootCommandCompositesDto
            {
                Id = id,
                CompositeAttr = compositeAttr,
                SomeDate = someDate,
                Composites = composites,
                Composite = composite
            };
        }
    }
}