using Intent.RoslynWeaver.Attributes;
using Wolverine.AspNetCore.Controllers.Domain.Common;
using Wolverine.AspNetCore.Controllers.Domain.Events;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Wolverine.AspNetCore.Controllers.Domain.Entities
{
    public class Product : IHasDomainEvent
    {
        public Product()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public bool IsActive { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];

        public void ChangeProduct(string name, decimal price)
        {
            Name = name;
            Price = price;
            DomainEvents.Add(new ProductChangedDomainEvent(
                product: this));
        }
    }
}