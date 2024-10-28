using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public class UpdateAggregateRootCommandAggregateDto1
    {
        public UpdateAggregateRootCommandAggregateDto1()
        {
            AggregationAttr = null!;
        }

        public Guid Id { get; set; }
        public string AggregationAttr { get; set; }

        public static UpdateAggregateRootCommandAggregateDto1 Create(Guid id, string aggregationAttr)
        {
            return new UpdateAggregateRootCommandAggregateDto1
            {
                Id = id,
                AggregationAttr = aggregationAttr
            };
        }
    }
}