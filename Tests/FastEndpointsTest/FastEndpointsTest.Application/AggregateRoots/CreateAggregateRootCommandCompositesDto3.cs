using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public class CreateAggregateRootCommandCompositesDto3
    {
        public CreateAggregateRootCommandCompositesDto3()
        {
            CompositeAttr = null!;
            Composites = null!;
        }

        public string CompositeAttr { get; set; }
        public DateTime? SomeDate { get; set; }
        public List<CreateAggregateRootCommandCompositesDto4> Composites { get; set; }

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