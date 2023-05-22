using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRootLongs
{

    public class CreateAggregateRootLongCompositeOfAggrLongDto
    {
        public CreateAggregateRootLongCompositeOfAggrLongDto()
        {
        }

        public static CreateAggregateRootLongCompositeOfAggrLongDto Create(string attribute)
        {
            return new CreateAggregateRootLongCompositeOfAggrLongDto
            {
                Attribute = attribute
            };
        }

        public string Attribute { get; set; } = null!;

    }
}