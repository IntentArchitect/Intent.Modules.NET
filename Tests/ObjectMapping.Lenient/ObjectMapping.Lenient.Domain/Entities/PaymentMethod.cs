using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace ObjectMapping.Lenient.Domain.Entities
{
    public class PaymentMethod
    {
        public PaymentMethod()
        {
            Label = null!;
        }

        public Guid Id { get; set; }

        public string Label { get; set; }

        public Guid OrderId { get; set; }
    }
}