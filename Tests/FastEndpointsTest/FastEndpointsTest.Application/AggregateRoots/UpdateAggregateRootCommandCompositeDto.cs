using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public class UpdateAggregateRootCommandCompositeDto
    {
        public UpdateAggregateRootCommandCompositeDto()
        {
            CompositeAttr = null!;
        }

        public Guid Id { get; set; }
        public string CompositeAttr { get; set; }

        public static UpdateAggregateRootCommandCompositeDto Create(Guid id, string compositeAttr)
        {
            return new UpdateAggregateRootCommandCompositeDto
            {
                Id = id,
                CompositeAttr = compositeAttr
            };
        }
    }
}