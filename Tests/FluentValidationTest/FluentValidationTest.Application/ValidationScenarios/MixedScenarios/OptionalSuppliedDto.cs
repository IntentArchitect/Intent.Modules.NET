using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.MixedScenarios
{
    public record OptionalSuppliedDto
    {
        public OptionalSuppliedDto()
        {
            Value = null!;
        }

        public string Value { get; init; }
    }
}