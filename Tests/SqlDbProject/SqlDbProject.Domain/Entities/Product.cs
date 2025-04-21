using Intent.RoslynWeaver.Attributes;
using SqlDbProject.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace SqlDbProject.Domain.Entities
{
    public class Product : IHasDomainEvent
    {
        public Product()
        {
            CountryIso = null!;
            Name = null!;
            Description = null!;
            ProductRatingConfig = null!;
            Country = null!;
        }

        public int ProductId { get; set; }

        public string CountryIso { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsDefault { get; set; }

        public int RatingMethodId { get; set; }

        public string ProductRatingConfig { get; set; }

        public virtual Country Country { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}