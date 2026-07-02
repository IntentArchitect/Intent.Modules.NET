using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Subscribe.MassTransit.DomainInteractionsRepro.Domain.Entities
{
    public class CatalogueItem
    {
        public CatalogueItem()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Sequence { get; set; }

        public Guid CatalogueId { get; set; }
    }
}