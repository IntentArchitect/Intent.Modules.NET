using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootCompositeManyBS
{

    public class CompositeManyBBDTO
    {
        public CompositeManyBBDTO()
        {
        }

        public static CompositeManyBBDTO Create(
            Guid id,
            string compositeAttr,
            Guid aCompositeManyId)
        {
            return new CompositeManyBBDTO
            {
                Id = id,
                CompositeAttr = compositeAttr,
                ACompositeManyId = aCompositeManyId,
            };
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public Guid ACompositeManyId { get; set; }

    }
}