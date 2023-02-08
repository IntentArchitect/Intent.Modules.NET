using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRoots
{

    public class CreateAggregateRootAggregateSingleCDto
    {
        public CreateAggregateRootAggregateSingleCDto()
        {
        }

        public static CreateAggregateRootAggregateSingleCDto Create(
            string aggregationAttr)
        {
            return new CreateAggregateRootAggregateSingleCDto
            {
                AggregationAttr = aggregationAttr,
            };
        }

        public string AggregationAttr { get; set; }

    }
}