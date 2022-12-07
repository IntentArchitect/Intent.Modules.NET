using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootLongs
{

    public class UpdateAggregateRootLongCompositeOfAggrLongDTO
    {
        public UpdateAggregateRootLongCompositeOfAggrLongDTO()
        {
        }

        public static UpdateAggregateRootLongCompositeOfAggrLongDTO Create(
            long id,
            string attribute)
        {
            return new UpdateAggregateRootLongCompositeOfAggrLongDTO
            {
                Id = id,
                Attribute = attribute,
            };
        }

        public long Id { get; set; }

        public string Attribute { get; set; }

    }
}