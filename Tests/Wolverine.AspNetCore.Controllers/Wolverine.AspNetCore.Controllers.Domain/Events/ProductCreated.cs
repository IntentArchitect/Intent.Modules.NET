using Intent.RoslynWeaver.Attributes;
using Wolverine.AspNetCore.Controllers.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Domain.Events
{
    public class ProductCreated : DomainEvent
    {
        public ProductCreated()
        {
        }
    }
}