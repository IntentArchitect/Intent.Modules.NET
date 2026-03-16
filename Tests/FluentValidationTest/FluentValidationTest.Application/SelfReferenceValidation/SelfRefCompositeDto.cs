using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FluentValidationTest.Application.SelfReferenceValidation
{
    public record SelfRefCompositeDto
    {
        public SelfRefCompositeDto()
        {
            Entry = null!;
        }

        public string Entry { get; init; }
    }
}