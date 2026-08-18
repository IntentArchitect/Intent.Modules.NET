using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace ObjectMapping.Lenient.Application.Orders
{
    public record CardPaymentDto
    {
        public CardPaymentDto()
        {
            Label = null!;
            CardLast4 = null!;
        }

        public Guid Id { get; init; }
        public string Label { get; init; }
        public string CardLast4 { get; init; }
    }
}