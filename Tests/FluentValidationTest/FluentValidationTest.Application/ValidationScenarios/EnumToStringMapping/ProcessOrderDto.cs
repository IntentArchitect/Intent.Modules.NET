using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.EnumToStringMapping
{
    public record ProcessOrderDto
    {
        public ProcessOrderDto()
        {
            Notes = null!;
        }

        public OrderStatus Status { get; init; }
        public string Notes { get; init; }
        public Process Process { get; init; }
    }
}