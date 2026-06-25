using Intent.RoslynWeaver.Attributes;
using Wolverine.AspNetCore.Controllers.Domain.Common;
using Wolverine.AspNetCore.Controllers.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Domain.Events
{
    public class ProductChangedDomainEvent : DomainEvent
    {
        public ProductChangedDomainEvent(Product product)
        {
            Product = product;
        }

        public Product Product { get; }
    }
}