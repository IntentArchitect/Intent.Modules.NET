using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public class UpdateAggregateRootCommandCompositesDto3
    {
        public UpdateAggregateRootCommandCompositesDto3()
        {
            CompositeAttr = null!;
            Composites = null!;
        }

        public Guid Id { get; set; }
        public string CompositeAttr { get; set; }
        public DateTime? SomeDate { get; set; }
        public List<UpdateAggregateRootCommandCompositesDto4> Composites { get; set; }

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