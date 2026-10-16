using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace TestMediator.Publisher.Domain
{
    public enum OrderStatus
    {
        Placed,
        Shipped
    }
}