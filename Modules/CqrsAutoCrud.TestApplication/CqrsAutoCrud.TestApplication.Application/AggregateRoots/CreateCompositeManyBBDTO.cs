using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRoots
{

    public class CreateCompositeManyBBDTO
    {
        public CreateCompositeManyBBDTO()
        {
        }

        public static CreateCompositeManyBBDTO Create(
            Guid id,
            string compositeAttr,
            Guid aCompositeManyId)
        {
            return new CreateCompositeManyBBDTO
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