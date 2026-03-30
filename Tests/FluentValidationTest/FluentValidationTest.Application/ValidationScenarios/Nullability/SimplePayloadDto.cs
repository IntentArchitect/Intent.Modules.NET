using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.Nullability
{
    public record SimplePayloadDto
    {
        public SimplePayloadDto()
        {
            PayloadName = null!;
        }

        public string PayloadName { get; init; }
        public string? OptionalPayloadCode { get; init; }
    }
}