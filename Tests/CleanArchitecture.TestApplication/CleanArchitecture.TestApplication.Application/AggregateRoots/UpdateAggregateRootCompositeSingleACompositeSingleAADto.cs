using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRoots
{

    public class UpdateAggregateRootCompositeSingleACompositeSingleAADto
    {
        public UpdateAggregateRootCompositeSingleACompositeSingleAADto()
        {
        }

        public static UpdateAggregateRootCompositeSingleACompositeSingleAADto Create(
            string compositeAttr,
            Guid id)
        {
            return new UpdateAggregateRootCompositeSingleACompositeSingleAADto
            {
                CompositeAttr = compositeAttr,
                Id = id,
            };
        }

        public string CompositeAttr { get; set; }

        public Guid Id { get; set; }

    }
}