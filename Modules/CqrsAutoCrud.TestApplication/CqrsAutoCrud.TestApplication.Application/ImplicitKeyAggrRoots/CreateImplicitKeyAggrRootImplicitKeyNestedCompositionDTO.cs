using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.ImplicitKeyAggrRoots
{

    public class CreateImplicitKeyAggrRootImplicitKeyNestedCompositionDTO
    {
        public CreateImplicitKeyAggrRootImplicitKeyNestedCompositionDTO()
        {
        }

        public static CreateImplicitKeyAggrRootImplicitKeyNestedCompositionDTO Create(
            string attribute)
        {
            return new CreateImplicitKeyAggrRootImplicitKeyNestedCompositionDTO
            {
                Attribute = attribute,
            };
        }

        public string Attribute { get; set; }

    }
}