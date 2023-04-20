using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots
{

    public class CreateImplicitKeyAggrRootImplicitKeyNestedCompositionDto
    {
        public CreateImplicitKeyAggrRootImplicitKeyNestedCompositionDto()
        {
        }

        public static CreateImplicitKeyAggrRootImplicitKeyNestedCompositionDto Create(string attribute)
        {
            return new CreateImplicitKeyAggrRootImplicitKeyNestedCompositionDto
            {
                Attribute = attribute
            };
        }

        public string Attribute { get; set; }

    }
}