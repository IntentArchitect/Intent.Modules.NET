using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.ContractEnumModel", Version = "1.0")]

namespace ObjectMapping.Lenient.Application.Orders
{
    public enum OrderStatusDto
    {
        Draft,
        Submitted,
        Shipped,
        Cancelled
    }
}