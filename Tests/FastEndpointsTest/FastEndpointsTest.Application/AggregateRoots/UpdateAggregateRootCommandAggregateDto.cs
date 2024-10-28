using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public class UpdateAggregateRootCommandAggregateDto
    {
        public UpdateAggregateRootCommandAggregateDto()
        {
            AggregationAttr = null!;
        }

        public Guid Id { get; set; }
        public string AggregationAttr { get; set; }

        public static UpdateAggregateRootCommandAggregateDto Create(Guid id, string aggregationAttr)
        {
            return new UpdateAggregateRootCommandAggregateDto
            {
                Id = id,
                AggregationAttr = aggregationAttr
            };
        }
    }
}