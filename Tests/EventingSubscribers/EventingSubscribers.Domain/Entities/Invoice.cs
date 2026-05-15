using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EventingSubscribers.Domain.Entities
{
    /// <summary>
    /// Create Entity with Value Object address
    /// </summary>
    public class Invoice
    {
        public Invoice()
        {
            Description = null!;
            BillingAddress = null!;
        }

        public Guid Id { get; set; }

        public string Description { get; set; }

        public Address BillingAddress { get; set; }
    }
}