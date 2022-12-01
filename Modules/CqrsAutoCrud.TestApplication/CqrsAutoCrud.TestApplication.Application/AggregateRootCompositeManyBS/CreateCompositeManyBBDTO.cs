using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootCompositeManyBS
{

    public class CreateCompositeManyBBDTO
    {
        public CreateCompositeManyBBDTO()
        {
        }

        public static CreateCompositeManyBBDTO Create(
            string compositeAttr,
            Guid aCompositeManyId)
        {
            return new CreateCompositeManyBBDTO
            {
                CompositeAttr = compositeAttr,
                ACompositeManyId = aCompositeManyId,
            };
        }

        public string CompositeAttr { get; set; }

        public Guid ACompositeManyId { get; set; }

    }
}