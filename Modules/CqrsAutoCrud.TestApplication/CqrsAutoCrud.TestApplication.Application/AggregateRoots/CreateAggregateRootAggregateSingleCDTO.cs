using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRoots
{

    public class CreateAggregateRootAggregateSingleCDTO
    {
        public CreateAggregateRootAggregateSingleCDTO()
        {
        }

        public static CreateAggregateRootAggregateSingleCDTO Create(
            string aggregationAttr)
        {
            return new CreateAggregateRootAggregateSingleCDTO
            {
                AggregationAttr = aggregationAttr,
            };
        }

        public string AggregationAttr { get; set; }

    }
}