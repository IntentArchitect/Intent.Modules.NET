using Intent.RoslynWeaver.Attributes;
using Wolverine.AspNetCore.FastEndpoints.Domain.Common;
using Wolverine.AspNetCore.FastEndpoints.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace Wolverine.AspNetCore.FastEndpoints.Domain.Events
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