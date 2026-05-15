using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities.Sales;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities
{
    public class CustomerSegments
    {
        public CustomerSegments()
        {
            Segment = null!;
            Customer = null!;
        }

        public Guid Id { get; set; }

        public Guid SegmentId { get; set; }

        public Guid CustomerId { get; set; }

        public decimal Confidence { get; set; }

        public virtual Segment Segment { get; set; }

        public virtual Customer Customer { get; set; }
    }
}