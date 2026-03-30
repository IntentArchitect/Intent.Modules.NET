using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.RecursiveDtos
{
    public record RecursiveNodeDto
    {
        public RecursiveNodeDto()
        {
            Name = null!;
        }

        public string Name { get; init; }
        public int Level { get; init; }
        public string? OptionalCode { get; init; }
        public RecursiveNodeDto? Child { get; init; }
    }
}