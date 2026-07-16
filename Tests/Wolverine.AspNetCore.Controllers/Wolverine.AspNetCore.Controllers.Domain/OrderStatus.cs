using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Domain
{
    public enum OrderStatus
    {
        Pending,
        Confirmed,
        Shipped,
        Cancelled
    }
}