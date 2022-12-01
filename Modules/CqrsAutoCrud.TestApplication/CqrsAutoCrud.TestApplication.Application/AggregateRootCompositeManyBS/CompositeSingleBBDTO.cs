using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootCompositeManyBS
{

    public class CompositeSingleBBDTO
    {
        public CompositeSingleBBDTO()
        {
        }

        public static CompositeSingleBBDTO Create(
            Guid id,
            string compositeAttr)
        {
            return new CompositeSingleBBDTO
            {
                Id = id,
                CompositeAttr = compositeAttr,
            };
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

    }
}