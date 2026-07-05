using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Subscribe.MassTransit.DomainInteractionsRepro.Domain.Entities
{
    public class Catalogue
    {
        public Catalogue()
        {
            Name = null!;
            Code = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public virtual ICollection<CatalogueItem>? CatalogueItems { get; set; } = [];
    }
}