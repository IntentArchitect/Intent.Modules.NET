using AspNetCoreCleanArchitecture.Sample.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AspNetCoreCleanArchitecture.Sample.Domain.Entities
{
    public class Product : IHasDomainEvent
    {
        public Product()
        {
            Name = null!;
            Description = null!;
            ImageUrl = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}