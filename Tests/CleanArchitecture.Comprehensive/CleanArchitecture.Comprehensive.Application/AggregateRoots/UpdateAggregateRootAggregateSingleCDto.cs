using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots
{
    public class UpdateAggregateRootAggregateSingleCDto
    {
        public UpdateAggregateRootAggregateSingleCDto()
        {
            AggregationAttr = null!;
        }

        public string AggregationAttr { get; set; }
        public Guid Id { get; set; }

        public static UpdateAggregateRootAggregateSingleCDto Create(string aggregationAttr, Guid id)
        {
            return new UpdateAggregateRootAggregateSingleCDto
            {
                AggregationAttr = aggregationAttr,
                Id = id
            };
        }
    }
}