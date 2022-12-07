using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.ImplicitKeyAggrRoots
{

    public class UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionDTO
    {
        public UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionDTO()
        {
        }

        public static UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionDTO Create(
            string attribute,
            Guid id)
        {
            return new UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionDTO
            {
                Attribute = attribute,
                Id = id,
            };
        }

        public string Attribute { get; set; }

        public Guid Id { get; set; }

    }
}