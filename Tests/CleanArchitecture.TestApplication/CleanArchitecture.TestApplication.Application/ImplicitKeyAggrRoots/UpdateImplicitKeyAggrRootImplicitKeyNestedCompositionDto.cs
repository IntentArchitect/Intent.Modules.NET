using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots
{
    public class UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionDto
    {
        public UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionDto()
        {
            Attribute = null!;
        }

        public string Attribute { get; set; }
        public Guid Id { get; set; }

        public static UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionDto Create(string attribute, Guid id)
        {
            return new UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionDto
            {
                Attribute = attribute,
                Id = id
            };
        }
    }
}