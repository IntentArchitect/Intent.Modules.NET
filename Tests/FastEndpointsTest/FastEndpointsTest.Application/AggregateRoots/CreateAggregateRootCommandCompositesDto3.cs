using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public record CreateAggregateRootCommandCompositesDto3
    {
        public CreateAggregateRootCommandCompositesDto3()
        {
            CompositeAttr = null!;
            Composites = null!;
        }

        public string CompositeAttr { get; init; }
        public DateTime? SomeDate { get; init; }
        public List<CreateAggregateRootCommandCompositesDto4> Composites { get; init; }

        public static CreateAggregateRootCommandCompositesDto3 Create(
            string compositeAttr,
            DateTime? someDate,
            List<CreateAggregateRootCommandCompositesDto4> composites)
        {
            return new CreateAggregateRootCommandCompositesDto3
            {
                CompositeAttr = compositeAttr,
                SomeDate = someDate,
                Composites = composites
            };
        }
    }
}