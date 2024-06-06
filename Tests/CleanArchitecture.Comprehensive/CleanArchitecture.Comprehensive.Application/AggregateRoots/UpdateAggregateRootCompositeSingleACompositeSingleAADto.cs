using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots
{
    public class UpdateAggregateRootCompositeSingleACompositeSingleAADto
    {
        public UpdateAggregateRootCompositeSingleACompositeSingleAADto()
        {
            CompositeAttr = null!;
        }

        public string CompositeAttr { get; set; }
        public Guid Id { get; set; }

        public static UpdateAggregateRootCompositeSingleACompositeSingleAADto Create(string compositeAttr, Guid id)
        {
            return new UpdateAggregateRootCompositeSingleACompositeSingleAADto
            {
                CompositeAttr = compositeAttr,
                Id = id
            };
        }
    }
}