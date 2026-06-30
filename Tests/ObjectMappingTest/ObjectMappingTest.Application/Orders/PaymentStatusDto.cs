using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.ContractEnumModel", Version = "1.0")]

namespace ObjectMappingTest.Application.Orders
{
    public enum PaymentStatusDto
    {
        Pending,
        Paid,
        Refunded
    }
}