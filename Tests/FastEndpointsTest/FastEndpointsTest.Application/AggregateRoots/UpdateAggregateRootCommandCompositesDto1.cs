using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public class UpdateAggregateRootCommandCompositesDto1
    {
        public UpdateAggregateRootCommandCompositesDto1()
        {
            CompositeAttr = null!;
        }

        public Guid Id { get; set; }
        public string CompositeAttr { get; set; }

        public static UpdateAggregateRootCommandCompositesDto1 Create(Guid id, string compositeAttr)
        {
            return new UpdateAggregateRootCommandCompositesDto1
            {
                Id = id,
                CompositeAttr = compositeAttr
            };
        }
    }
}