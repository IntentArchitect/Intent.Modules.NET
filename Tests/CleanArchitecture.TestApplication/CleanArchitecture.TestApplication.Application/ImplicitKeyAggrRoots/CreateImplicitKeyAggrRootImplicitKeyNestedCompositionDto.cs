using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots
{
    public class CreateImplicitKeyAggrRootImplicitKeyNestedCompositionDto
    {
        public CreateImplicitKeyAggrRootImplicitKeyNestedCompositionDto()
        {
            Attribute = null!;
        }

        public string Attribute { get; set; }

        public static CreateImplicitKeyAggrRootImplicitKeyNestedCompositionDto Create(string attribute)
        {
            return new CreateImplicitKeyAggrRootImplicitKeyNestedCompositionDto
            {
                Attribute = attribute
            };
        }
    }
}