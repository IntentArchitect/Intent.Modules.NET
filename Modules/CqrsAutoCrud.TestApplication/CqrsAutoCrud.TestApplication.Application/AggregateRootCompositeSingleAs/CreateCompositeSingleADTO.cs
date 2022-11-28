using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootCompositeSingleAs
{

    public class CreateCompositeSingleADTO
    {
        public CreateCompositeSingleADTO()
        {
        }

        public static CreateCompositeSingleADTO Create(
            Guid id,
            string compositeAttr,
            CreateCompositeSingleAADTO? composite,
            List<CreateCompositeManyAADTO> composites)
        {
            return new CreateCompositeSingleADTO
            {
                Id = id,
                CompositeAttr = compositeAttr,
                Composite = composite,
                Composites = composites,
            };
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public CreateCompositeSingleAADTO? Composite { get; set; }

        public List<CreateCompositeManyAADTO> Composites { get; set; }
    }
}