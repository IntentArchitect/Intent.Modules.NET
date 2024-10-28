using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public class UpdateAggregateRootCommandCompositesDto4
    {
        public UpdateAggregateRootCommandCompositesDto4()
        {
            CompositeAttr = null!;
        }

        public Guid Id { get; set; }
        public string CompositeAttr { get; set; }

        public static UpdateAggregateRootCommandCompositesDto4 Create(Guid id, string compositeAttr)
        {
            return new UpdateAggregateRootCommandCompositesDto4
            {
                Id = id,
                CompositeAttr = compositeAttr
            };
        }
    }
}