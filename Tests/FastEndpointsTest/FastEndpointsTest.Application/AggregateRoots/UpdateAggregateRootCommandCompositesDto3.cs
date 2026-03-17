using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public record UpdateAggregateRootCommandCompositesDto3
    {
        public UpdateAggregateRootCommandCompositesDto3()
        {
            CompositeAttr = null!;
            Composites = null!;
        }

        public Guid Id { get; init; }
        public string CompositeAttr { get; init; }
        public DateTime? SomeDate { get; init; }
        public List<UpdateAggregateRootCommandCompositesDto4> Composites { get; init; }

        public static UpdateAggregateRootCommandCompositesDto3 Create(
            Guid id,
            string compositeAttr,
            DateTime? someDate,
            List<UpdateAggregateRootCommandCompositesDto4> composites)
        {
            return new UpdateAggregateRootCommandCompositesDto3
            {
                Id = id,
                CompositeAttr = compositeAttr,
                SomeDate = someDate,
                Composites = composites
            };
        }
    }
}