using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRootLongs
{

    public class UpdateAggregateRootLongCompositeOfAggrLongDto
    {
        public UpdateAggregateRootLongCompositeOfAggrLongDto()
        {
        }

        public static UpdateAggregateRootLongCompositeOfAggrLongDto Create(string attribute, long id)
        {
            return new UpdateAggregateRootLongCompositeOfAggrLongDto
            {
                Attribute = attribute,
                Id = id,
            };
        }

        public string Attribute { get; set; }

        public long Id { get; set; }

    }
}