using Intent.RoslynWeaver.Attributes;
using MudBlazor.Sample.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MudBlazor.Sample.Domain.Entities
{
    public class Product : IHasDomainEvent
    {
        public Product()
        {
            Name = null!;
            Description = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}