using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootCompositeSingleAs
{

    public class UpdateCompositeSingleADTO
    {
        public UpdateCompositeSingleADTO()
        {
        }

        public static UpdateCompositeSingleADTO Create(
            Guid id,
            string compositeAttr,
            UpdateCompositeSingleAADTO? composite,
            List<UpdateCompositeManyAADTO> composites)
        {
            return new UpdateCompositeSingleADTO
            {
                Id = id,
                CompositeAttr = compositeAttr,
                Composite = composite,
                Composites = composites,
            };
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public UpdateCompositeSingleAADTO? Composite { get; set; }

        public List<UpdateCompositeManyAADTO> Composites { get; set; }
    }
}