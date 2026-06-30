using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace ObjectMappingTest.Application.Orders
{
    public record OrderSummaryDto
    {
        public OrderSummaryDto()
        {
            RefNo = null!;
            DisplayName = null!;
        }

        public Guid Id { get; init; }
        public string RefNo { get; init; }
        public PaymentStatusDto PaymentStatus { get; init; }
        public string DisplayName { get; init; }
    }
}