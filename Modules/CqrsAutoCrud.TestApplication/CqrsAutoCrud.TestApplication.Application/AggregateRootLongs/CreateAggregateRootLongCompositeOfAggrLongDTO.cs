using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootLongs
{

    public class CreateAggregateRootLongCompositeOfAggrLongDTO
    {
        public CreateAggregateRootLongCompositeOfAggrLongDTO()
        {
        }

        public static CreateAggregateRootLongCompositeOfAggrLongDTO Create(
            string attribute)
        {
            return new CreateAggregateRootLongCompositeOfAggrLongDTO
            {
                Attribute = attribute,
            };
        }

        public string Attribute { get; set; }

    }
}